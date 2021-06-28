using Autofac;
using Eiffel.Persistence.Abstractions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Eiffel.Persistence.Tenancy
{
    public class DatabasePerTenantStrategy<TContext> : ITenancyStrategy<TContext>
        where TContext : DbContext
    {
        private readonly ContainerBuilder _containerBuilder;
        private readonly TenancyDbContext _dbContext;

        public DatabasePerTenantStrategy(ContainerBuilder containerBuilder, TenancyDbContext dbContext)
        {
            _containerBuilder = containerBuilder ?? throw new ArgumentNullException(nameof(containerBuilder));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public Abstractions.TenancyStrategy Strategy => Abstractions.TenancyStrategy.Databse;

        public virtual void Execute()
        {
            var tenants = _dbContext.Tenants.ToList();

            foreach (var tenant in tenants)
            {
                var optionsBuilder = new DbContextOptionsBuilder<TContext>();
                optionsBuilder.UseSqlServer(tenant.ConnectionString);
                
                _containerBuilder.RegisterType<TContext>()
                    .WithParameter("options", optionsBuilder.Options)
                    .Keyed<TContext>(tenant.Id)
                    .InstancePerLifetimeScope();

                var tenantMetadata = new TenantMetadata
                {
                    Id = tenant.Id,
                    ExpiresOn = tenant.ExpiresOn,
                    IsEnabled = tenant.IsEnabled,
                    Name = tenant.Name,
                    Value = tenant.ConnectionString
                };

                _containerBuilder.RegisterInstance(tenantMetadata)
                    .Keyed<ITenantMetadata>(tenant.Id)
                    .SingleInstance();
            }
        }
    }
}
