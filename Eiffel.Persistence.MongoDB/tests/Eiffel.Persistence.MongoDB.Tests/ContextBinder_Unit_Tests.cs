using Eiffel.Persistence.MongoDB.Abstractions;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Moq;
using System.Collections.Generic;
using System.Threading;
using Xunit;

namespace Eiffel.Persistence.MongoDB.Tests
{
    public class ContextBinder_Unit_Tests
    {
        [Fact]
        public void ContextBinder_Should_Bind_Collection()
        {
            // Arrange
            var services = new ServiceCollection();
            var _documents = new List<MockUserCollection>();

            var collectionBuilder = new MockCollectionBuilder<MockUserCollection>(_documents);
            var mockCollection = collectionBuilder
                .WithEmptyCollection()
                .WithAggregate()
                .WithCreate()
                .WithRead()
                .WithUpdate()
                .WithDelete()
                .WithCount()
                .Build();

            var mockDatabase = new MockDatabaseBuilder<MockUserCollection>(mockCollection.Object)
                .WithCollections(new string[0])
                .Build();

            var mockClient = new MockClientBuilder(mockDatabase.Object).Build();

            var mockOptions = new Mock<DbContextOptions<MockDbContext>>(new MongoClientSettings()) { CallBase = true };
            var mockContext = new Mock<MockDbContext>(new[] { mockOptions.Object }) { CallBase = true };

            mockContext.SetupGet(x => x.Database).Returns(mockDatabase.Object);
            mockContext.SetupGet(x => x.Client).Returns(mockClient.Object);

            var typeBuilder = new CollectionTypeBuilder<MockUserCollection>();
            var collectionConfig = new MockCollectionTypeConfiguration();
            collectionConfig.Configure(typeBuilder);

            services.AddSingleton<ICollectionTypeConfiguration<MockUserCollection>>(collectionConfig);
            services.AddSingleton(mockContext.Object);
            var serviceProvider = services.BuildServiceProvider();

            // Act
            DbContextBinder<MockDbContext>.Bind(mockContext.Object, serviceProvider);
            var context = serviceProvider.GetRequiredService<MockDbContext>();

            // Assert
            mockDatabase.Verify(x => x.CreateCollection(It.IsAny<string>(), It.IsAny<CreateCollectionOptions>(), It.IsAny<CancellationToken>()), Times.Once);
            mockDatabase.Verify(x => x.GetCollection<MockUserCollection>(It.IsAny<string>(), It.IsAny<MongoCollectionSettings>()), Times.Once);
            mockClient.Verify(x => x.GetDatabase(It.IsAny<string>(), It.IsAny<MongoDatabaseSettings>()), Times.Once);
            mockContext.Verify(x => x.Database, Times.AtLeastOnce());
            context.Users.Should().NotBeNull();
        }
    }
}
