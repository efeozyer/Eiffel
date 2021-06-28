using Autofac;
using Eiffel.Persistence.Abstractions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Eiffel.Persistence.Tenancy
{
    public class SchemaPerTenantStrategy<TContext> : ITenancyStrategy<TContext>
        where TContext : DbContext
    {
        private readonly ContainerBuilder _containerBuilder;
        private readonly TenancyDbContext _dbContext;
        private readonly IDefaultConnectionStringProvider _connectionStringProvider;

        public SchemaPerTenantStrategy(ContainerBuilder containerBuilder, TenancyDbContext dbContext, IDefaultConnectionStringProvider connectionStringProvider)
        {
            _containerBuilder = containerBuilder ?? throw new ArgumentNullException(nameof(containerBuilder));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _connectionStringProvider = connectionStringProvider;
        }

        public Abstractions.TenancyStrategy Strategy => Abstractions.TenancyStrategy.Schema;

        public virtual void Execute()
        {
            var connectionString = _connectionStringProvider.Get();

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentNullException($"Connection string could not be <{connectionString}>. Checkout documentation");
            }

            var contextType = typeof(TContext);

            if(contextType.GetProperty("Schema") == null)
            {
                throw new MissingMemberException($"{contextType.Name} does not contains Schema property.");
            }

            var optionsBuilder = new DbContextOptionsBuilder<TContext>();
            optionsBuilder.UseSqlServer(connectionString);

            var tenants = _dbContext.Tenants
              .ToList();

            foreach (var tenant in tenants)
            {
                _containerBuilder.RegisterType<TContext>()
                    .WithParameter("options", optionsBuilder.Options)
                    .WithProperty("Schema", tenant.SchemaName)
                    .Keyed<TContext>(tenant.Id)
                    .InstancePerLifetimeScope();

                var tenantMetadata = new TenantMetadata
                {
                    Id = tenant.Id,
                    ExpiresOn = tenant.ExpiresOn,
                    IsEnabled = tenant.IsEnabled,
                    Name = tenant.Name,
                    Value = tenant.SchemaName
                };

                _containerBuilder.RegisterInstance(tenantMetadata)
                    .Keyed<ITenantMetadata>(tenant.Id)
                    .SingleInstance();
            }
        }
    }
}
