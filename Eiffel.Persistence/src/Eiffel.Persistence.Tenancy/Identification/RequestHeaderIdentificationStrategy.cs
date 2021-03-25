using Eiffel.Persistence.Abstractions;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;

namespace Eiffel.Persistence.Tenancy
{
    public class RequestHeaderIdentificationStrategy : ITenantIdentificationStrategy
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly TenancyDbContext _dbContext;

        public RequestHeaderIdentificationStrategy(IHttpContextAccessor httpContextAccessor, TenancyDbContext dbContext)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public ITenantMetadata Identify()
        {
            if (!_httpContextAccessor.HttpContext.Request.Headers.ContainsKey("tenantId")) 
            {
                throw new TenantIdentificationFailedException("RequestHeaderIdentificationStrategy requires tenantId in request headers.");
            }

            var tenantId = _httpContextAccessor.HttpContext.Request.Headers["tenantId"];
            var tenant = _dbContext.Tenants.FirstOrDefault(x => x.Id == tenantId);

            if (tenant == null)
            {
                throw new TenantIdentificationFailedException($"Invalid tenant id : {tenantId}.");
            }

            return new TenantMetadata
            {
                Id = tenantId,
                Name = tenant.Name,
                ExpiresOn = tenant.ExpiresOn,
                IsEnabled = tenant.IsEnabled,
                Value = tenant.Value
            };
        }
    }
}
