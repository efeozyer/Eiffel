using Eiffel.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Threading.Tasks;

namespace Eiffel.Web.AspNetCore
{
    public class ExceptionLocalizationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IExceptionLocalizationService _localizationService;

        public ExceptionLocalizationMiddleware(RequestDelegate next, IExceptionLocalizationService localizationService)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _localizationService = localizationService ?? throw new ArgumentNullException(nameof(localizationService));
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (DomainException ex)
            {
                context.Request.Headers.TryGetValue("Accept-Language", out StringValues culture);

                var message = _localizationService.Get(ex, culture.ToString(), ex.Parameters);

                var exception = (DomainException)Activator.CreateInstance(ex.GetType(), message);

                throw exception;
            }
        }
    }
}
