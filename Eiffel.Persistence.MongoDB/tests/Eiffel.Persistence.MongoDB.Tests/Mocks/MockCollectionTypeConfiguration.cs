using Eiffel.Persistence.MongoDB.Abstractions;

namespace Eiffel.Persistence.MongoDB.Tests
{
    public class MockCollectionTypeConfiguration : ICollectionTypeConfiguration<MockUserCollection>
    {
        public void Configure(ICollectionTypeBuilder<MockUserCollection> builder)
        {
            builder.ToCollection("Users");
        }
    }
}
