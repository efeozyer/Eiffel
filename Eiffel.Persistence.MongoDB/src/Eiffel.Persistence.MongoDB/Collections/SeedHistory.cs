using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Eiffel.Persistence.MongoDB.Collections
{
    public class SeedHistory
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string Database { get; set; }
        public string CollectionName { get; set; }
        public long DocumentCount { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
