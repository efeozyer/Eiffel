namespace Eiffel.Persistence.Abstractions
{
    public interface ITenantIdentificationStrategy
    {
        ITenantMetadata Identify();
    }
}
