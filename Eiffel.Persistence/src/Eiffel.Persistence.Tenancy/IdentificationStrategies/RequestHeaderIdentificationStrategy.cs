using Eiffel.Persistence.Abstractions;
using Microsoft.AspNetCore.Http;
using System;

namespace Eiffel.Persistence.Tenancy
{
    public class RequestHeaderIdentificationStrategy : ITenantIdentificationStrategy<string>
    {
        protected readonly IHttpContextAccessor HttpContextAccessor;

        public RequestHeaderIdentificationStrategy(IHttpContextAccessor httpContextAccessor)
        {
            HttpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public string Name => "Request Header Identification Strategy";

        public virtual string Identify()
        {
            if (!HttpContextAccessor.HttpContext.Request.Headers.ContainsKey("tenantId")) 
            {
                throw new TenantIdentificationFailedException("Request header tenant identification failed");
            }

            var tenantId = HttpContextAccessor.HttpContext.Request.Headers["tenantId"];

            return tenantId;
        }
    }
}
