using Autofac;
using Eiffel.Persistence.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Eiffel.Persistence.Tenancy
{
    public static class ContainerBuilderextensions
    {
        public static ContainerBuilder AddTenancy<TContext, TStrategy>(this ContainerBuilder services, TStrategy tenancyStrategy)
            where TContext : DbContext
            where TStrategy : class, ITenancyStrategy<TContext>
        {
            return services;
        }
    }
}
