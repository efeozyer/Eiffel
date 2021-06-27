using Eiffel.Persistence.Abstractions.Entity;
using System;

namespace Eiffel.Persistence.Tenancy
{
    public class TenantEntity : Entity<string>
    {
        public string Name { get; set; }
        public DateTime ExpiresOn { get; set; }
        public bool IsEnabled { get; set; }
        public string TablePrefix { get; set; }
        public string SchemaName { get; set; }
        public string ConnectionString { get; set; }
    }
}
