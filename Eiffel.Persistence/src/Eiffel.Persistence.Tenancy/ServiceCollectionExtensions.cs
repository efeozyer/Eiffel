using Eiffel.Persistence.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Eiffel.Persistence.Tenancy
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddTenancy<TContext, TStrategy>(this IServiceCollection services, TStrategy tenancyStrategy, ITenantIdentificationStrategy)
            where TContext : DbContext
            where TStrategy : class, ITenancyStrategy<TContext>
        {

            return services;
        }
    }
}
