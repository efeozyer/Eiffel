using MongoDB.Bson;

namespace Eiffel.Persistence.MongoDb.Tests.Mocks
{
    public class MockUserCollection
    {
        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public byte Age { get; set; }
    }
}
