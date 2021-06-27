using System;

namespace Eiffel.Persistence.Abstractions
{
    public interface ITenantMetadata
    {
        string Id { get; }
        string Name { get; }
        bool IsEnabled { get; }
        DateTime ExpiresOn { get; }
        string Value { get; }
    }
}
