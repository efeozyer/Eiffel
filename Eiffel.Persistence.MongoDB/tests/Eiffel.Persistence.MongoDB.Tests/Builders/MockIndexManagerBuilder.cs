using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Eiffel.Persistence.MongoDB.Tests
{
    public class MockIndexManagerBuilder<TDocument>
        where TDocument : class
    {
        private readonly Mock<IMongoIndexManager<TDocument>> _indexManager;
        private readonly List<CreateIndexModel<TDocument>> _indexModels;

        public MockIndexManagerBuilder(List<CreateIndexModel<TDocument>> indexModels = null)
        {
            _indexManager = new Mock<IMongoIndexManager<TDocument>>() { CallBase = true };
            _indexModels = indexModels ?? new List<CreateIndexModel<TDocument>>();
        }

        public MockIndexManagerBuilder<TDocument> WithCreate()
        {
            _indexManager.Setup(x => x.CreateOne(It.IsAny<CreateIndexModel<TDocument>>(), It.IsAny<CreateOneIndexOptions>(), It.IsAny<CancellationToken>()))
                .Callback((CreateIndexModel<TDocument> indexModel, CreateOneIndexOptions options, CancellationToken cancellationToken) =>
                {
                    _indexModels.Add(indexModel);
                })
                .Returns(string.Empty);

            _indexManager.Setup(x => x.CreateOneAsync(It.IsAny<CreateIndexModel<TDocument>>(), It.IsAny<CreateOneIndexOptions>(), It.IsAny<CancellationToken>()))
                .Callback((CreateIndexModel<TDocument> indexModel, CreateOneIndexOptions options, CancellationToken cancellationToken) =>
                {
                    _indexModels.Add(indexModel);
                })
               .Returns(Task.FromResult(string.Empty));

            _indexManager.Setup(x => x.CreateMany(It.IsAny<IEnumerable<CreateIndexModel<TDocument>>>(), It.IsAny<CreateManyIndexesOptions>(), It.IsAny<CancellationToken>()))
                .Callback((IEnumerable<CreateIndexModel<TDocument>> indexModels, CreateManyIndexesOptions options, CancellationToken cancellationToken) =>
                {
                    _indexModels.AddRange(indexModels);
                })
                .Returns(Enumerable.Empty<string>());

            _indexManager.Setup(x => x.CreateManyAsync(It.IsAny<IEnumerable<CreateIndexModel<TDocument>>>(), It.IsAny<CreateManyIndexesOptions>(), It.IsAny<CancellationToken>()))
                 .Callback((IEnumerable<CreateIndexModel<TDocument>> indexModels, CreateManyIndexesOptions options, CancellationToken cancellationToken) =>
                 {
                     _indexModels.AddRange(indexModels);
                 })
                .Returns(Task.FromResult(Enumerable.Empty<string>()));
            return this;
        }

        public MockIndexManagerBuilder<TDocument> WithList()
        {
            var indexModels = _indexModels.Select(x => x.ToBsonDocument()).ToList();
            _indexManager.Setup(x => x.List(It.IsAny<CancellationToken>()))
                .Returns(() =>
                {
                    return MockCursorBuilder<BsonDocument>.Build(indexModels);
                });

            _indexManager.Setup(x => x.ListAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(() =>
                {
                    return MockCursorBuilder<BsonDocument>.Build(indexModels);
                });
            return this;
        }

        public Mock<IMongoIndexManager<TDocument>> Build()
        {
            return _indexManager;
        }
    }
}
