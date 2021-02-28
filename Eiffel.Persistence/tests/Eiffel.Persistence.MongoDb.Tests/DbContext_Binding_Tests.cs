using Eiffel.Persistence.MongoDb.Tests.Mocks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using MongoDB.Driver.Core.Bindings;
using MongoDB.Driver.Core.Operations;
using MongoDB.Driver.Core.WireProtocol.Messages.Encoders;
using Moq;
using System;
using System.Linq;
using System.Threading;
using Xunit;

namespace Eiffel.Persistence.MongoDb.Tests
{
    public class DbContext_Binding_Tests
    {
        private readonly IServiceCollection _services;

        public DbContext_Binding_Tests()
        {
            _services = new ServiceCollection();
        }

        [Fact]
        public void DocumentContext_Binder_Should_Bind_Collections()
        {
            // Arrange
            var collectionNamespace = new CollectionNamespace("test", "test");
            var collectionSettings = new MongoCollectionSettings();
            var documentSerializer = collectionSettings.SerializerRegistry.GetSerializer<MockUserCollection>();
            var asyncCursor = CreateCursor(documentSerializer, 1);

            var mockCollection = new Mock<IMongoCollection<MockUserCollection>> { DefaultValue = DefaultValue.Mock, CallBase = true };
            mockCollection.SetupGet(s => s.DocumentSerializer).Returns(documentSerializer);
            mockCollection.SetupGet(s => s.Settings).Returns(collectionSettings);
            mockCollection.SetupGet(x => x.CollectionNamespace).Returns(collectionNamespace);
            mockCollection.Setup(x => x.Aggregate(It.IsAny<PipelineDefinition<MockUserCollection, MockUserCollection>>(), It.IsAny<AggregateOptions>(), It.IsAny<CancellationToken>())).Returns(asyncCursor);
            mockCollection.Setup(x => x.AggregateAsync(It.IsAny<PipelineDefinition<MockUserCollection, MockUserCollection>>(), It.IsAny<AggregateOptions>(), It.IsAny<CancellationToken>())).ReturnsAsync(asyncCursor);

            var mockDatabase = new Mock<IMongoDatabase>();
            mockDatabase.SetupGet(x => x.DatabaseNamespace).Returns(new DatabaseNamespace("test"));
            mockDatabase.Setup(x => x.GetCollection<MockUserCollection>(It.IsAny<string>(), It.IsAny<MongoCollectionSettings>())).Returns(mockCollection.Object);

            var mockClient = new Mock<IMongoClient>();
            mockClient.Setup(x => x.GetDatabase(It.IsAny<string>(), It.IsAny<MongoDatabaseSettings>())).Returns(mockDatabase.Object);

            var mockOptions = new Mock<DbContextOptions<MockDbContext>>(new MongoClientSettings()) { CallBase = true };
            var mockContext = new Mock<MockDbContext>(new[] { mockOptions.Object }) { CallBase = true };
            mockContext.SetupGet(x => x.Database).Returns(mockDatabase.Object);
            mockContext.SetupGet(x => x.Client).Returns(mockClient.Object);

            var mockConfig = new Mock<ICollectionTypeConfiguration<MockUserCollection>>();
            mockConfig.SetupGet(x => x.Name).Returns("Users");
            mockConfig.SetupGet(x => x.CollectionSettings).Returns(new MongoCollectionSettings());
            _services.AddSingleton(mockConfig.Object);

            _services.AddSingleton(mockContext.Object);
            var serviceProvider = _services.BuildServiceProvider();

            // Act
            DbContextBinder<MockDbContext>.Bind(mockContext.Object, serviceProvider);
            var context = serviceProvider.GetRequiredService<MockDbContext>();

            // Assert
            mockDatabase.Verify(x => x.GetCollection<MockUserCollection>(It.IsAny<string>(), It.IsAny<MongoCollectionSettings>()), Times.Once);
            mockClient.Verify(x => x.GetDatabase(It.IsAny<string>(), It.IsAny<MongoDatabaseSettings>()), Times.Once);
            mockContext.Verify(x => x.Database, Times.AtLeastOnce());
            context.Users.Should().NotBeNull();
        }

        private IAsyncCursor<TDocument> CreateCursor<TDocument>(IBsonSerializer<TDocument> serializer, int count)
        {
            var firstBatch = Enumerable.Range(0, count)
                .Select(i => Activator.CreateInstance<TDocument>())
                .ToArray();

                return new AsyncCursor<TDocument>(
                channelSource: new Mock<IChannelSource>() { CallBase =  true }.Object,
                collectionNamespace: new CollectionNamespace("test", "test"),
                query: new BsonDocument(),
                firstBatch: firstBatch,
                cursorId: 0,
                batchSize: null,
                limit: null,
                serializer: serializer,
                messageEncoderSettings: new MessageEncoderSettings(),
                maxTime: null);
        }
    }
}
