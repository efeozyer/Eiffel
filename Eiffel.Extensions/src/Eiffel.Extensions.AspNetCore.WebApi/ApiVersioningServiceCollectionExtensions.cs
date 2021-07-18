using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Eiffel.Extensions.AspNetCore.WebApi
{
    public static class ApiVersioningServiceCollectionExtensions
    {
        public static void AddVersioning(this IServiceCollection services, ApiVersion defaultVersion = null)
        {
            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = defaultVersion ?? new ApiVersion(1, 0);
                options.ReportApiVersions = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
            });

            services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });
        }
    }
}
