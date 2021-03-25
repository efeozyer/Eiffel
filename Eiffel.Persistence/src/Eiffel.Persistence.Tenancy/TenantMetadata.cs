using Eiffel.Persistence.Abstractions;
using System;

namespace Eiffel.Persistence.Tenancy
{
    public class TenantMetadata : ITenantMetadata
    {
        public string Id { get; internal set; }
        public string Name { get; internal set; }
        public bool IsEnabled { get; internal set; }
        public DateTime ExpiresOn { get; internal set; }
        public string Value { get; internal set; }
    }
}
