using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace Eiffel.Extensions.Swagger
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Requires IApiVersionDescriptionProvider
        /// </summary>
        public static void AddSwagger(this IServiceCollection services)
        {
            var applicationName = Assembly.GetCallingAssembly().GetName().Name;

            services.AddSwaggerGen(x =>
            {
                var serviceProvider = services.BuildServiceProvider();
                var provider = serviceProvider.GetRequiredService<IApiVersionDescriptionProvider>();
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    x.SwaggerDoc(description.GroupName, new OpenApiInfo { Title = $"{applicationName} version {description.ApiVersion}", Version = description.GroupName });
                }
            });
        }
    }
}
