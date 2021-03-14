using Eiffel.Persistence.MongoDB.Abstractions;
using Eiffel.Persistence.Samples.MongoDB.Collections;

namespace Eiffel.Persistence.Samples.MongoDB.Configurations
{
    public class ProfileCollectinTypeConfiguration : ICollectionTypeConfiguration<Profile>
    {
        public void Configure(ICollectionTypeBuilder<Profile> builder)
        {
            builder.ToCollection("Profiles");
        }
    }
}
