using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;

namespace Eiffel.Extensions.AspNetCore.WebApi
{
    public static class ApiEndPointApplicationBuilderExtensions
    {
        /// <summary>
        /// Requires api verisoning!
        /// </summary>
        public static void UseDefaultApiInfoProvider(this IApplicationBuilder app)
        {
            var applicationName = Assembly.GetCallingAssembly().GetName().Name;

            var provider = app.ApplicationServices.GetRequiredService<IApiVersionDescriptionProvider>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {

                    var apiInfo = new ApiInfo(applicationName, provider);

                    var response = JsonSerializer.Serialize(apiInfo);

                    await context.Response.WriteAsync(response);

                });
            });
        }
    }

    internal class ApiInfo
    {
        public string ApplicationName { get; set; }
        public List<VersionInfo> SupportedVersions { get; set; }

        public ApiInfo(string applicationName, IApiVersionDescriptionProvider provider)
        {
            SupportedVersions = new List<VersionInfo>();

            ApplicationName = applicationName;

            foreach (var description in provider.ApiVersionDescriptions)
            {
                SupportedVersions.Add(new VersionInfo
                {
                    Version = description.GroupName,
                    IsDeprecated = description.IsDeprecated,
                });
            }
        }
    }

    internal class VersionInfo
    {
        public string Version { get; set; }

        public bool IsDeprecated { get; set; }
    }
}
