using System;

namespace Eiffel.Persistence.Abstractions
{
    public interface ITenantValidator<TKey>
        where TKey : IEquatable<TKey>
    {
        void Validate(TKey key);
    }
}
