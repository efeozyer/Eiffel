using Eiffel.AspNetCore.WebApi;
using Microsoft.AspNetCore.Builder;
using System;

namespace Eiffel.Extensions.AspNetCore.WebApi
{
    public static class ApiResponseWrapperApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseApiResponseWrapper(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            return app.UseMiddleware<ApiResponseWrapperMiddleware>();
        }
    }
}
