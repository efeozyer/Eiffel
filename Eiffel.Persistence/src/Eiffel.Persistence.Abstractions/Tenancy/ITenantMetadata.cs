using System;

namespace Eiffel.Persistence.Abstractions
{
    public interface ITenantMetadata
    {
        string Id { get; set; }
        string Name { get; set; }
        bool IsEnabled { get; set; }
        DateTime ExpiresOn { get; set; }
    }
}
