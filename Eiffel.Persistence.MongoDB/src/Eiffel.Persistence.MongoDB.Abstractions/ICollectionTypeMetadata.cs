using MongoDB.Driver;

namespace Eiffel.Persistence.MongoDB.Abstractions
{
    public interface ICollectionTypeMetadata
    {
        string CollectionName { get; }
        dynamic Documents { get; }
        dynamic IndexKeys { get; }
        dynamic FilterExpression { get; }
        DocumentValidationAction? ValidationAction { get; }
        DocumentValidationLevel? ValidationLevel { get; }
        bool? IsCapped { get; }
        long? MaxSize { get; }
        long? MaxDocuments { get; }
        dynamic CollectionOptions { get; }
        MongoCollectionSettings ColletionSettings { get; }
    }
}
