using Microsoft.AspNetCore.Builder;
using System;

namespace Eiffel.Web.AspNetCore
{
    public static class ExceptionLocalizationApplicationBuilderExtensions
    {
        /// <summary>
        /// Requires IExceptionLocalizationService
        /// </summary>
        public static IApplicationBuilder UseExceptionLocalization(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            return app.UseMiddleware<ExceptionLocalizationMiddleware>();
        }
    }
}
