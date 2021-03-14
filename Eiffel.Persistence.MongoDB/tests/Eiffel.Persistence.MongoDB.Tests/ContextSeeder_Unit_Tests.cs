using Eiffel.Persistence.MongoDB.Abstractions;
using Eiffel.Persistence.MongoDB.Collections;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Moq;
using System.Collections.Generic;
using System.Threading;
using Xunit;

namespace Eiffel.Persistence.MongoDB.Tests
{
    public class ContextSeeder_Unit_Tests
    {
        private readonly Mock<IMongoCollection<User>> _mockCollection;
        private readonly Mock<IMongoDatabase> _mockDatabase;
        private readonly Mock<UserDbContext> _mockContext;
        private readonly IServiceCollection _services;

        public ContextSeeder_Unit_Tests()
        {
            _services = new ServiceCollection();
            CreateMockObjects(ref _mockCollection, ref _mockDatabase, ref _mockContext);
        }

        [Fact]
        public void ContextTypeBuilder_Should_Seed_Collection()
        {
            // Arrange
            var documents = new List<User>()
            {
                User.Create("Test", 1, false),
                User.Create("Test", 2, true)
            };

            var collectionBuilder = new MockCollectionBuilder<User>();
            var mockCollection = collectionBuilder
                .WithEmptyCollection()
                .WithCreate()
                .WithRead()
                .WithCount()
                .Build();

            var mockDatabase = new MockDatabaseBuilder<User>(mockCollection.Object)
               .WithCollectionNames(new string[0])
               .Build();

            mockDatabase.Setup(x => x.CreateCollection(It.IsAny<string>(), It.IsAny<CreateCollectionOptions>(), It.IsAny<CancellationToken>()));
            mockDatabase.Setup(x => x.GetCollection<SeedHistory>("__SeedHistory", It.IsAny<MongoCollectionSettings>()))
                .Returns(() =>
                {
                    return new MockCollectionBuilder<SeedHistory>().WithEmptyCollection().Build().Object;
                });

            var mockClient = new MockClientBuilder(mockDatabase.Object).Build();
            var mockOptions = new Mock<DbContextOptions<UserDbContext>>(new MongoClientSettings()) { CallBase = true };
            var mockContext = new Mock<UserDbContext>(new[] { mockOptions.Object }) { CallBase = true };

            mockContext.SetupGet(x => x.Database).Returns(mockDatabase.Object);
            mockContext.SetupGet(x => x.Client).Returns(mockClient.Object);

            var mockConfig = new Mock<UserCollectionTypeConfiguration>() { CallBase = true };
            mockConfig.Setup(x => x.Configure(It.IsAny<ICollectionTypeBuilder<User>>()))
                .Callback((ICollectionTypeBuilder<User> builder) =>
                {
                    builder.ToCollection("Users");
                    builder.HasDocuments(documents);
                });

            _services.AddSingleton<ICollectionTypeConfiguration<User>>(mockConfig.Object);
            _services.AddSingleton(mockContext.Object);
            var serviceProvider = _services.BuildServiceProvider();
            DbContextBinder<UserDbContext>.Bind(mockContext.Object, serviceProvider);
            DbContextSeeder<UserDbContext>.Seed(mockContext.Object, serviceProvider);

            // Act
            var users = mockContext.Object.Users.Find(x => true).ToList();

            // Assert
            mockCollection.Verify(x => x.InsertMany(It.IsAny<IEnumerable<User>>(), It.IsAny<InsertManyOptions>(), It.IsAny<CancellationToken>()), Times.Once);
            users.Count.Should().Be(documents.Count);
        }

        private void CreateMockObjects<TDocument, TContext>(
           ref Mock<IMongoCollection<TDocument>> mockCollection,
           ref Mock<IMongoDatabase> mockDatabase,
           ref Mock<TContext> mockContext
       )
           where TDocument : class
           where TContext : DbContext
        {
            var _documents = new List<TDocument>();

            var collectionBuilder = new MockCollectionBuilder<TDocument>(_documents);
            mockCollection = collectionBuilder
                .WithEmptyCollection()
                .Build();

            mockDatabase = new MockDatabaseBuilder<TDocument>(mockCollection.Object)
                .WithCollectionNames(new string[0])
                .Build();

            var mockClient = new MockClientBuilder(mockDatabase.Object).Build();
            var mockOptions = new Mock<DbContextOptions<UserDbContext>>(new MongoClientSettings()) { CallBase = true };
            mockContext = new Mock<TContext>(new[] { mockOptions.Object }) { CallBase = true };

            mockContext.SetupGet(x => x.Database).Returns(mockDatabase.Object);
            mockContext.SetupGet(x => x.Client).Returns(mockClient.Object);
        }
    }
}
