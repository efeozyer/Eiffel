using MongoDB.Driver;
using Moq;

namespace Eiffel.Persistence.MongoDB.Builders
{
    public class MockDatabaseBuilder<TDocument>
    {
        private readonly Mock<IMongoDatabase> _database;
        private readonly IMongoCollection<TDocument> _collection;

        public MockDatabaseBuilder(IMongoCollection<TDocument> collection)
        {
            _database = new Mock<IMongoDatabase>();
            _collection = collection;
        }

        public Mock<IMongoDatabase> Build()
        {
            _database.SetupGet(x => x.DatabaseNamespace).Returns(new DatabaseNamespace("test"));
            _database.Setup(x => x.GetCollection<TDocument>(It.IsAny<string>(), It.IsAny<MongoCollectionSettings>())).Returns(_collection);
            return _database;
        }
    }
}
