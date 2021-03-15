using Eiffel.Persistence.Abstractions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text.RegularExpressions;

namespace Eiffel.Persistence.Tenancy
{
    public delegate DbContext TenantDbContext();
    public class DatabasePerTenantStrategy<TContext> : ITenancyStrategy<TContext>
        where TContext : DbContext
    {
        private readonly DbContextOptions<TContext> _dbContextOptions;
        private readonly TenancyDbContext _dbContext;
        private readonly Dictionary<string, object> _contextInstaces = new Dictionary<string, object>();

        public DatabasePerTenantStrategy(DbContextOptions<TContext> dbContextOptions, TenancyDbContext dbContext)
        {
            _dbContextOptions = dbContextOptions ?? throw new ArgumentNullException(nameof(dbContextOptions));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public void Execute()
        {
            var tenants = _dbContext.Tenants.ToList();
            foreach(var tenant in tenants)
            {
                var context = ILContextBuilder<TContext>.Build(tenant.Name, _dbContextOptions);
                _contextInstaces.Add(tenant.Id, context);
            }
        }
    }

    internal static class ILContextBuilder<TContext>
        where TContext : DbContext
    {
        public static object Build(string name, DbContextOptions<TContext> contextOptions)
        {
            var className = Regex.Replace(name, "[^0-9a-zA-Z]+", "_", RegexOptions.IgnoreCase);
            var typeBuilder = GetTypeBuilder(className, typeof(TContext));
            var contextType = typeBuilder.CreateType();
            var instance = Activator.CreateInstance(contextType, new[] { contextOptions });
            return instance;
        }

        private static TypeBuilder GetTypeBuilder(string typeName, Type baseType = null)
        {
            var assemblyName = new AssemblyName(typeName);
            AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(Assembly.GetExecutingAssembly().GetModules().First().Name);
            TypeBuilder typeBuilder = moduleBuilder.DefineType(typeName,
                    TypeAttributes.Public |
                    TypeAttributes.Class |
                    TypeAttributes.AutoClass |
                    TypeAttributes.AnsiClass |
                    TypeAttributes.BeforeFieldInit |
                    TypeAttributes.AutoLayout,
                    baseType);
            return typeBuilder;
        }
    }
}
