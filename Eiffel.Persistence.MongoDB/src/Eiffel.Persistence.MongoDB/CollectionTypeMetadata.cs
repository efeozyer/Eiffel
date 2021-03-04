using Eiffel.Persistence.MongoDB.Abstractions;
using MongoDB.Driver;

namespace Eiffel.Persistence.MongoDB
{
    public class CollectionTypeMetadata : ICollectionTypeMetadata
    {
        public string CollectionName { get; internal set; }
        public dynamic Documents { get; internal set; }
        public DocumentValidationAction ValidationAction { get; internal set; }
        public DocumentValidationLevel ValidationLevel { get; internal set; }
        public bool? IsCapped { get; internal set; }
        public long? MaxSize { get; internal set; }
        public long? MaxDocuments { get; internal set; }
        public CreateCollectionOptions<object> CollectionOptions { get; internal set; }
        public MongoCollectionSettings ColletionSettings { get; internal set; }
        public dynamic IndexKeys { get; internal set; }
        public dynamic FilterExpression { get; internal set; }
    }
}
