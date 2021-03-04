using Eiffel.Persistence.MongoDB.Abstractions;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Xunit;

namespace Eiffel.Persistence.MongoDB.Tests
{
    public class CollectionTypeBuilder_Unit_Tests
    {
        private readonly Mock<IMongoCollection<User>> _mockCollection;
        private readonly Mock<IMongoDatabase> _mockDatabase;
        private readonly Mock<UserDbContext> _mockContext;
        private readonly IServiceCollection _services;

        public CollectionTypeBuilder_Unit_Tests()
        {
            _services = new ServiceCollection();
            CreateMockObjects(ref _mockCollection, ref _mockDatabase, ref _mockContext);
        }

        [Fact]
        public void ContextTypeBuilder_Should_Set_CollectionName()
        {
            // Arrange
            var collectionName = "Test";
            var mockConfig = new Mock<UserCollectionTypeConfiguration>() { CallBase = true };
            mockConfig.Setup(x => x.Configure(It.IsAny<ICollectionTypeBuilder<User>>())).Callback<ICollectionTypeBuilder<User>>((builder) =>
            {
                builder.ToCollection(collectionName);
            });

            _services.AddSingleton<ICollectionTypeConfiguration<User>>(mockConfig.Object);
            _services.AddSingleton(_mockContext.Object);
            var serviceProvider = _services.BuildServiceProvider();

            // Act
            DbContextBinder<UserDbContext>.Bind(_mockContext.Object, serviceProvider);

            // Assert
            _mockDatabase.Verify(x => x.CreateCollection(collectionName, It.IsAny<CreateCollectionOptions>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory]
        [InlineData(true, 1)]
        [InlineData(false, 2)]
        public void ContextTypeBuilder_Should_Set_QueryFilter(bool isDeleted, int expectedResult)
        {
            // Arrange
            var documents = new List<User>()
            {
                User.Create("Test", 1, false),
                User.Create("Test", 2, true),
                User.Create("Test", 3, false),
            };

            var mockCollection = new MockCollectionBuilder<User>(documents)
                .WithEmptyCollection()
                .WithRead()
                .Build();

            var mockDatabase = new MockDatabaseBuilder<User>(mockCollection.Object)
                .WithCollectionNames(new string[0])
                .Build();

            var mockClient = new MockClientBuilder(mockDatabase.Object).Build();
            var mockOptions = new Mock<DbContextOptions<UserDbContext>>(new MongoClientSettings()) { CallBase = true };
            var mockContext = new Mock<UserDbContext>(new[] { mockOptions.Object }) { CallBase = true };

            mockContext.SetupGet(x => x.Database).Returns(mockDatabase.Object);
            mockContext.SetupGet(x => x.Client).Returns(mockClient.Object);

            var mockConfig = new Mock<UserCollectionTypeConfiguration>() { CallBase = true };
            mockConfig.Setup(x => x.Configure(It.IsAny<ICollectionTypeBuilder<User>>())).Callback<ICollectionTypeBuilder<User>>((builder) =>
            {
                builder.HasQueryFilter(x => x.IsDeleted == isDeleted);
            });

            _services.AddSingleton<ICollectionTypeConfiguration<User>>(mockConfig.Object);
            _services.AddSingleton(mockContext.Object);
            var serviceProvider = _services.BuildServiceProvider();
            DbContextBinder<UserDbContext>.Bind(mockContext.Object, serviceProvider);

            // Act
            var users = mockContext.Object.Users.Find(x => x.Name == "Test").ToList();

            // Assert
            users.Count.Should().Be(expectedResult);
        }

        [Fact]
        public void ContextTypeBuilder_Should_Set_IndexKeys()
        {
            // Arrange
            var indexKeyModels = new List<CreateIndexModel<User>>();
            var mockIndexManager = new MockIndexManagerBuilder<User>(indexKeyModels)
                .WithCreate()
                .WithList()
                .Build();

            _mockCollection.SetupGet(x => x.Indexes).Returns(mockIndexManager.Object);

            var mockConfig = new Mock<UserCollectionTypeConfiguration>() { CallBase = true };
            mockConfig.Setup(x => x.Configure(It.IsAny<ICollectionTypeBuilder<User>>())).Callback<ICollectionTypeBuilder<User>>((builder) =>
            {
                builder
                    .HasIndex(x => x.CreatedOn, false)
                    .HasIndex(x => x.Name, false);
            });

            _services.AddSingleton<ICollectionTypeConfiguration<User>>(mockConfig.Object);
            _services.AddSingleton(_mockContext.Object);
            var serviceProvider = _services.BuildServiceProvider();

            // Act
            DbContextBinder<UserDbContext>.Bind(_mockContext.Object, serviceProvider);

            // Assert
            mockIndexManager.Verify(x => x.CreateOne(It.IsAny<CreateIndexModel<User>>(), It.IsAny<CreateOneIndexOptions>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
            indexKeyModels.Count.Should().Be(2);
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

            var mockClient = new MockClientBuilder(mockDatabase.Object).Build();
            var mockOptions = new Mock<DbContextOptions<UserDbContext>>(new MongoClientSettings()) { CallBase = true };
            var mockContext = new Mock<UserDbContext>(new[] { mockOptions.Object }) { CallBase = true };

            mockContext.SetupGet(x => x.Database).Returns(mockDatabase.Object);
            mockContext.SetupGet(x => x.Client).Returns(mockClient.Object);

            var mockConfig = new Mock<UserCollectionTypeConfiguration>() { CallBase = true };
            mockConfig.Setup(x => x.Configure(It.IsAny<ICollectionTypeBuilder<User>>()))
                .Callback((ICollectionTypeBuilder<User> builder) =>
                {
                    builder.HasDocuments(documents);
                });

            _services.AddSingleton<ICollectionTypeConfiguration<User>>(mockConfig.Object);
            _services.AddSingleton(mockContext.Object);
            var serviceProvider = _services.BuildServiceProvider();
            DbContextBinder<UserDbContext>.Bind(mockContext.Object, serviceProvider);

            // Act
            var users = mockContext.Object.Users.Find(x => true).ToList();

            // Assert
            mockCollection.Verify(x => x.InsertMany(It.IsAny<IEnumerable<User>>(), It.IsAny<InsertManyOptions>(), It.IsAny<CancellationToken>()), Times.Once);
            users.Count.Should().Be(documents.Count);
        }

        [Fact]
        public void ContextTypeBuilder_Should_Set_ValidationRules()
        {
            // Arrange
            var collectionBuilder = new MockCollectionBuilder<User>();
            var mockCollection = collectionBuilder
                .WithEmptyCollection()
                .WithCreate()
                .Build();

            var mockDatabase = new MockDatabaseBuilder<User>(mockCollection.Object)
               .WithCollectionNames(new string[0])
               .Build();

            var mockClient = new MockClientBuilder(mockDatabase.Object).Build();
            var mockOptions = new Mock<DbContextOptions<UserDbContext>>(new MongoClientSettings()) { CallBase = true };
            var mockContext = new Mock<UserDbContext>(new[] { mockOptions.Object }) { CallBase = true };

            mockContext.SetupGet(x => x.Database).Returns(mockDatabase.Object);
            mockContext.SetupGet(x => x.Client).Returns(mockClient.Object);

            var mockConfig = new Mock<UserCollectionTypeConfiguration>() { CallBase = true };
            mockConfig.Setup(x => x.Configure(It.IsAny<ICollectionTypeBuilder<User>>()))
                .Callback((ICollectionTypeBuilder<User> builder) =>
                {
                    builder.IsRequired(x => x.Name);
                });

            _services.AddSingleton<ICollectionTypeConfiguration<User>>(mockConfig.Object);
            _services.AddSingleton(mockContext.Object);
            var serviceProvider = _services.BuildServiceProvider();
            DbContextBinder<UserDbContext>.Bind(mockContext.Object, serviceProvider);

            // Act
            mockContext.Object.Users.Add(new User());

            // Assert
            mockCollection.Verify(x => x.InsertOne(It.IsAny<User>(), It.IsAny<InsertOneOptions>(), It.IsAny<CancellationToken>()), Times.Once);
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
