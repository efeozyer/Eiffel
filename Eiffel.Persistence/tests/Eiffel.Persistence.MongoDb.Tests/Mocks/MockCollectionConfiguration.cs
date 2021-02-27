using MongoDB.Driver;

namespace Eiffel.Persistence.MongoDb.Tests.Mocks
{
    public class MockUserCollectionConfiguration : ICollectionTypeConfiguration<MockUserCollection>
    {
        public string Name => "MockUserCollection";
        public CreateCollectionOptions CreateCollectionOptions => default;
        public MongoCollectionSettings CollectionSettings => default;
    }
}
