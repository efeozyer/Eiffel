using Eiffel.Persistence.Abstractions;
using MongoDB.Bson;

namespace Eiffel.Persistence.MongoDb.Tests.Mocks
{
    public class MockUserCollection : Document<ObjectId>
    {
        public int Grade { get; set; }
        public string Name { get; set; }
        public byte Age { get; set; }
    }
}
