using Autofac;
using Eiffel.Persistence.Abstractions;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Eiffel.Persistence.Tenancy
{
    public class MultiTenancyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ITenantIdentificationStrategy<string> _identificationStrategy;
        private readonly ILifetimeScope _lifetimeScope;

        public MultiTenancyMiddleware(RequestDelegate next, ITenantIdentificationStrategy<string> strategy, ILifetimeScope lifetimeScope)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _identificationStrategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
            _lifetimeScope = lifetimeScope ?? throw new ArgumentNullException(nameof(lifetimeScope));
        }

        public virtual async Task InvokeAsync(HttpContext context)
        {
            var tenantId = _identificationStrategy.Identify();
            
            ValidateTenant(tenantId);
            
            await _next(context);
        }

        public virtual void ValidateTenant(string tenantId)
        {
            if (string.IsNullOrEmpty(tenantId))
            {
                throw new TenantIdentificationFailedException($"Tenant identification failed TenantId: {tenantId}, Strategy : {_identificationStrategy.Name}");
            }

            using(var scope = _lifetimeScope.BeginLifetimeScope())
            {
                var dbContext = _lifetimeScope.Resolve<TenancyDbContext>();

                var tenant = dbContext.Tenants.FirstOrDefault(x => x.Id == tenantId);

                if (tenant == null)
                {
                    throw new TenantNotFoundException($"Invalid tenant id : {tenantId}");
                }

                if (tenant.IsDeleted)
                {
                    throw new TenantIdentificationFailedException($"Identification failed! Tenant is deleted TenantId: {tenantId}");
                }

                if (!tenant.IsEnabled)
                {
                    throw new TenantIdentificationFailedException($"Identification failed! Tenant is disabled TenantId: {tenantId}");
                }

                if (tenant.ExpiresOn < DateTime.UtcNow)
                {
                    throw new TenantRegistrationExpiredException($"Identification failed! Tenant is expired TenantId: {tenantId}, Expired On: {tenant.ExpiresOn:dd-MM-yyyy hh:mm:ss}");
                }
            }
        }
    }
}
