using Eiffel.Domain;

namespace Eiffel.Web.AspNetCore
{
    public interface IExceptionLocalizationService
    {
        string Get(DomainException ex, string culture);
        string Get(DomainException ex, string culture, params object[] parameters);
    }
}
