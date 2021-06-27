using System;

namespace Eiffel.Persistence.Abstractions
{
    public interface ITenantIdentificationStrategy<TKey>
        where TKey : IEquatable<TKey>
    {
        string Name { get; }
        TKey Identify();
    }
}
