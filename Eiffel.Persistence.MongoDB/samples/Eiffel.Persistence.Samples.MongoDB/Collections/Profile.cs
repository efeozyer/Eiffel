using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Eiffel.Persistence.Samples.MongoDB.Collections
{
    public class Profile
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public ObjectId UserId { get; set; }    
    }
}
