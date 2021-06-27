using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eiffel.Persistence.Tenancy
{
    public class TenantEntityConfiguration : IEntityTypeConfiguration<TenantEntity>
    {
        public void Configure(EntityTypeBuilder<TenantEntity> builder)
        {
            builder.ToTable("Tenants");

            builder.HasQueryFilter(x => !x.IsDeleted && x.IsEnabled);
        }
    }
}
