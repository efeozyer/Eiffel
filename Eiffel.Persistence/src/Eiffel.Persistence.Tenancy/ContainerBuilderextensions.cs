using Autofac;
using Eiffel.Persistence.Abstractions;
using Microsoft.EntityFrameworkCore;
using System;

namespace Eiffel.Persistence.Tenancy
{
    public static class ContainerBuilderextensions
    {
        public static ContainerBuilder AddTenancy<TContext, TStrategy>(this ContainerBuilder builder)
            where TContext : DbContext
            where TStrategy : class, ITenancyStrategy<TContext>
        {
            var strategyType = typeof(TStrategy).MakeGenericType(typeof(TContext));

            var tenancyStrategy = (ITenancyStrategy<TContext>)Activator.CreateInstance(strategyType, new DbContextOptions<TenancyDbContext>());

            tenancyStrategy.Execute();

            return builder;
        }
    }
}
