using MongoDB.Driver;
using Moq;

namespace Eiffel.Persistence.Shared.MockBuilders
{
    public class MockClientBuilder
    {
        private readonly Mock<IMongoClient> _client;
        private readonly IMongoDatabase _database;

        public MockClientBuilder(IMongoDatabase database)
        {
            _client = new Mock<IMongoClient>();
            _database = database;
        }

        public Mock<IMongoClient> Build()
        {
            _client.Setup(x => x.GetDatabase(It.IsAny<string>(), It.IsAny<MongoDatabaseSettings>())).Returns(_database);
            return _client;
        }
    }
}
