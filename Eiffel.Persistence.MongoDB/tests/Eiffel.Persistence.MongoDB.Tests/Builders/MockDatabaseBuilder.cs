using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using System.Linq;
using System.Threading;

namespace Eiffel.Persistence.MongoDB.Tests
{
    public class MockDatabaseBuilder<TDocument>
        where TDocument : class
    {
        private readonly Mock<IMongoDatabase> _database;
        private readonly IMongoCollection<TDocument> _collection;

        public MockDatabaseBuilder(IMongoCollection<TDocument> collection)
        {
            _database = new Mock<IMongoDatabase>();
            _collection = collection;
        }

        public MockDatabaseBuilder<TDocument> WithCollectionNames(string[] collectionNames)
        {
            _database.Setup(x => x.ListCollections(It.IsAny<ListCollectionsOptions>(), It.IsAny<CancellationToken>())).Returns(() =>
            {
                var collections = collectionNames.Select(x => BsonDocument.Create(new { name = x }));
                return MockCursorBuilder<BsonDocument>.Build(collections);
            });

            _database.Setup(x => x.ListCollectionsAsync(It.IsAny<ListCollectionsOptions>(), It.IsAny<CancellationToken>())).ReturnsAsync(() =>
            {
                var collections = collectionNames.Select(x => BsonDocument.Create(new { name = x }));
                return MockCursorBuilder<BsonDocument>.Build(collections);
            });
            return this;
        }

        public Mock<IMongoDatabase> Build()
        {
            _database.SetupGet(x => x.DatabaseNamespace).Returns(new DatabaseNamespace("test"));
            _database.Setup(x => x.GetCollection<TDocument>(It.IsAny<string>(), It.IsAny<MongoCollectionSettings>())).Returns(_collection);
            return _database;
        }
    }
}
