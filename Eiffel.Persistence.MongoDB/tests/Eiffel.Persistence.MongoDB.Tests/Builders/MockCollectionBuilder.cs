using MongoDB.Driver;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;

namespace Eiffel.Persistence.MongoDB.Tests
{
    public class MockCollectionBuilder<TDocument>
        where TDocument : class
    {
        private readonly Mock<IMongoCollection<TDocument>> _collection;
        private readonly List<TDocument> _documents;

        public MockCollectionBuilder()
        {
            _collection = new Mock<IMongoCollection<TDocument>> { DefaultValue = DefaultValue.Mock, CallBase = true };
            _documents = new List<TDocument>();
        }

        public MockCollectionBuilder(Mock<IMongoCollection<TDocument>> collection)
        {
            _collection = collection;
            _documents = new List<TDocument>();
        }

        public MockCollectionBuilder(Mock<IMongoCollection<TDocument>> collection, List<TDocument> documents)
        {
            _collection = collection;
            _documents = documents;
        }

        public MockCollectionBuilder(List<TDocument> documents)
        {
            _documents = documents;
            _collection = new Mock<IMongoCollection<TDocument>> { DefaultValue = DefaultValue.Mock, CallBase = true };
        }


        public MockCollectionBuilder<TDocument> WithEmptyCollection()
        {
            var collectionSettings = new MongoCollectionSettings();
            var documentSerializer = collectionSettings.SerializerRegistry.GetSerializer<TDocument>();
            var collectionNamespace = new CollectionNamespace("test", "test");

            _collection.SetupGet(s => s.DocumentSerializer).Returns(documentSerializer);
            _collection.SetupGet(s => s.Settings).Returns(collectionSettings);
            _collection.SetupGet(x => x.CollectionNamespace).Returns(collectionNamespace);
            return this;
        }

        public MockCollectionBuilder<TDocument> WithRead()
        {
            _collection.Setup(x =>
                x.FindSync(It.IsAny<FilterDefinition<TDocument>>(), It.IsAny<FindOptions<TDocument>>(), It.IsAny<CancellationToken>()))
                    .Returns((FilterDefinition<TDocument> definition, FilterDefinition<TDocument> options, CancellationToken cancellationToken) =>
                    {
                        if (FilterDefinition<TDocument>.Empty == definition || definition == null)
                        {
                            return MockCursorBuilder<TDocument>.Build(_documents);
                        }

                        Expression<Func<TDocument, bool>> expression = ((dynamic)definition).Expression;
                        return MockCursorBuilder<TDocument>.Build(_documents.Where(expression.Compile()));
                    });

            _collection.Setup(x =>
                x.FindAsync(It.IsAny<FilterDefinition<TDocument>>(), It.IsAny<FindOptions<TDocument>>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync((FilterDefinition<TDocument> definition, FilterDefinition<TDocument> options, CancellationToken cancellationToken) =>
                    {
                        if (FilterDefinition<TDocument>.Empty == definition || definition == null)
                        {
                            return MockCursorBuilder<TDocument>.Build(_documents);
                        }

                        Expression<Func<TDocument, bool>> expression = ((dynamic)definition).Expression;
                        return MockCursorBuilder<TDocument>.Build(_documents.Where(expression.Compile()));
                    });

            return this;
        }
        public MockCollectionBuilder<TDocument> WithAggregate()
        {
            _collection.Setup(x =>
               x.Aggregate(It.IsAny<PipelineDefinition<TDocument, TDocument>>(), It.IsAny<AggregateOptions>(), It.IsAny<CancellationToken>()))
                   .Returns((PipelineDefinition<TDocument, TDocument> pipeline, AggregateOptions options, CancellationToken cancellationToken) =>
                   {
                       // TODO: Aggregate pipeline stages via Linq
                       return MockCursorBuilder<TDocument>.Build(_documents);
                   });

            _collection.Setup(x =>
                x.AggregateAsync(It.IsAny<PipelineDefinition<TDocument, TDocument>>(), It.IsAny<AggregateOptions>(), It.IsAny<CancellationToken>()))
                   .ReturnsAsync((PipelineDefinition<TDocument, TDocument> pipeline, AggregateOptions options, CancellationToken cancellationToken) =>
                   {
                       // TODO: Aggregate pipeline stages via Linq
                       return MockCursorBuilder<TDocument>.Build(_documents);
                   });

            return this;
        }
        public MockCollectionBuilder<TDocument> WithCreate()
        {
            _collection.Setup(x =>
               x.InsertMany(It.IsAny<IEnumerable<TDocument>>(), It.IsAny<InsertManyOptions>(), It.IsAny<CancellationToken>()))
                   .Callback((IEnumerable<TDocument> documents, InsertManyOptions options, CancellationToken cancellationToken) =>
                   {
                       _documents.AddRange(documents);
                   });

            _collection.Setup(x =>
               x.InsertManyAsync(It.IsAny<IEnumerable<TDocument>>(), It.IsAny<InsertManyOptions>(), It.IsAny<CancellationToken>()))
                   .Callback((IEnumerable<TDocument> documents, InsertManyOptions options, CancellationToken cancellationToken) =>
                   {
                       _documents.AddRange(documents);
                   });

            _collection.Setup(x =>
                x.InsertOne(It.IsAny<TDocument>(), It.IsAny<InsertOneOptions>(), It.IsAny<CancellationToken>()))
                   .Callback((TDocument document, InsertOneOptions options, CancellationToken cancellationToken) =>
                   {
                       _documents.Add(document);
                   });

            _collection.Setup(x =>
                x.InsertOneAsync(It.IsAny<TDocument>(), It.IsAny<InsertOneOptions>(), It.IsAny<CancellationToken>()))
                   .Callback((TDocument document, InsertOneOptions options, CancellationToken cancellationToken) =>
                   {
                       _documents.Add(document);
                   });
            return this;
        }
        public MockCollectionBuilder<TDocument> WithUpdate()
        {
            _collection.Setup(x =>
               x.ReplaceOne(It.IsAny<FilterDefinition<TDocument>>(), It.IsAny<TDocument>(), It.IsAny<ReplaceOptions>(), It.IsAny<CancellationToken>()))
                   .Callback((FilterDefinition<TDocument> definition, TDocument document, ReplaceOptions options, CancellationToken cancellationToken) =>
                   {
                       Expression<Func<TDocument, bool>> expression = ((dynamic)definition).Expression;
                       var record = _documents.FirstOrDefault(expression.Compile());
                       _documents.Remove(record);
                       _documents.Add(document);
                   });

            _collection.Setup(x =>
               x.ReplaceOneAsync(It.IsAny<FilterDefinition<TDocument>>(), It.IsAny<TDocument>(), It.IsAny<ReplaceOptions>(), It.IsAny<CancellationToken>()))
                   .Callback((FilterDefinition<TDocument> definition, TDocument document, ReplaceOptions options, CancellationToken cancellationToken) =>
                   {
                       Expression<Func<TDocument, bool>> expression = ((dynamic)definition).Expression;
                       var record = _documents.FirstOrDefault(expression.Compile());
                       _documents.Remove(record);
                       _documents.Add(document);
                   });
            return this;
        }
        public MockCollectionBuilder<TDocument> WithDelete()
        {
            _collection.Setup(x =>
              x.DeleteOne(It.IsAny<FilterDefinition<TDocument>>(), It.IsAny<CancellationToken>()))
                 .Callback((FilterDefinition<TDocument> definition, CancellationToken cancellationToken) =>
                 {
                     Expression<Func<TDocument, bool>> expression = ((dynamic)definition).Expression;
                     var record = _documents.FirstOrDefault(expression.Compile());
                     _documents.Remove(record);
                 });

            _collection.Setup(x =>
              x.DeleteOneAsync(It.IsAny<FilterDefinition<TDocument>>(), It.IsAny<CancellationToken>()))
                 .Callback((FilterDefinition<TDocument> definition, CancellationToken cancellationToken) =>
                 {
                     Expression<Func<TDocument, bool>> expression = ((dynamic)definition).Expression;
                     var record = _documents.FirstOrDefault(expression.Compile());
                     _documents.Remove(record);
                 });

            return this;
        }
        public MockCollectionBuilder<TDocument> WithCount()
        {
            _collection.Setup(x =>
              x.CountDocuments(It.IsAny<FilterDefinition<TDocument>>(), It.IsAny<CountOptions>(), It.IsAny<CancellationToken>()))
                  .Returns((FilterDefinition<TDocument> definition, CountOptions options, CancellationToken cancellationToken) =>
                  {
                      if (FilterDefinition<TDocument>.Empty == definition)
                      {
                          return _documents.Count;
                      }

                      Expression<Func<TDocument, bool>> expression = ((dynamic)definition).Expression;
                      return _documents.Count(expression.Compile());
                  });

            _collection.Setup(x =>
             x.CountDocumentsAsync(It.IsAny<FilterDefinition<TDocument>>(), It.IsAny<CountOptions>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync((FilterDefinition<TDocument> definition, CountOptions options, CancellationToken cancellationToken) =>
                 {
                     if (FilterDefinition<TDocument>.Empty == definition)
                     {
                         return _documents.Count;
                     }

                     Expression<Func<TDocument, bool>> expression = ((dynamic)definition).Expression;
                     return _documents.Count(expression.Compile());
                 });

            return this;
        }

        public Mock<IMongoCollection<TDocument>> Build()
        {
            return _collection;
        }
    }
}
