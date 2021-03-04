using Eiffel.Persistence.MongoDB.Abstractions;

namespace Eiffel.Persistence.MongoDB.Tests
{
    public class UserCollectionTypeConfiguration : ICollectionTypeConfiguration<User>
    {
        public virtual void Configure(ICollectionTypeBuilder<User> builder)
        {
            builder.ToCollection("Users");
        }
    }
}
