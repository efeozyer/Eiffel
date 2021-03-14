using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Eiffel.Persistence.Samples.MongoDB.Collections
{
    public class User
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public bool IsDeleted { get; set; }
    }
}
