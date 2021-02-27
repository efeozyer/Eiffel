using MongoDB.Driver;

namespace Eiffel.Persistence.MongoDb
{
    public interface ICollectionTypeConfiguration<TDocument>
        where TDocument : class
    {
        string Name { get; }

        CreateCollectionOptions CreateCollectionOptions { get; }
        MongoCollectionSettings CollectionSettings { get; }
    }
}
