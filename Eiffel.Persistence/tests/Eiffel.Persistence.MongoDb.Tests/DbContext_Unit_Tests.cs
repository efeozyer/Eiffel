using Eiffel.Persistence.MongoDb.Tests.Mocks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using Xunit;

namespace Eiffel.Persistence.MongoDb.Tests
{
    public class DbContext_Unit_Tests
    {
        private readonly MockDbContext _dbContext;
        public DbContext_Unit_Tests()
        {
            
            _dbContext = CreateDbContext();
        }

        [Fact]
        public void Should_Insert_Document()
        {
            // Arrange
            var user = CreateUser(1, 1, "test1");
            var users = new[]
            {
                CreateUser(1, 2, "test2"),
                CreateUser(1, 3, "test3")
            };

            // Act
            _dbContext.Users.Add(user);
            _dbContext.Users.AddRange(users);

            // Assert
            _dbContext.Users.Count().Should().Be(3);
        }

        [Fact]
        public void Should_Update_Document()
        {
            // Arrange
            _dbContext.Users.AddRange(new[]
            {
                CreateUser(1, 2, "test2"),
                CreateUser(1, 3, "test3")
            });

            var user = new MockUserCollection
            {
                Grade = 1,
                Age = 2,
                Name = "test"
            };

            // Act
            _dbContext.Users.Update(x => x.Age == 2, user);

            // Assert
            _dbContext.Users.Count().Should().Be(2);
            _dbContext.Users.Count(x => x.Name == "test").Should().Be(1);
            _dbContext.Users.Count(x => x.Name != "test").Should().Be(1);
        }

        [Fact]
        public void Should_Delete_Document()
        {
            // Arrange
            _dbContext.Users.AddRange(new[]
            {
                CreateUser(1, 2, "test2"),
                CreateUser(1, 3, "test3")
            });

            // Act
            _dbContext.Users.Delete(x => x.Age == 2);

            // Assert
            _dbContext.Users.Count().Should().Be(1);
        }

        [Fact]
        public void Should_Find_Document()
        {
            // Arrange
            _dbContext.Users.AddRange(new[]
            {
                CreateUser(1, 2, "test2"),
                CreateUser(1, 3, "test3")
            });

            // Act
            var user = _dbContext.Users.First(x => x.Age == 2);

            // Assert
            user.Grade.Should().Be(1);
            user.Age.Should().Be(2);
            user.Name.Should().Be("test2");
            var s = _dbContext.Users.AsQueryable();
        }

        private MockUserCollection CreateUser(int grade, byte age, string name)
        {
            return new MockUserCollection
            {
                Grade = grade,
                Age = age,
                Name = name
            };
        }

        private MockDbContext CreateDbContext()
        {
            var services = new ServiceCollection();
            var collectionNamespace = new CollectionNamespace("test", "test");
            var collectionSettings = new MongoCollectionSettings();
            var documentSerializer = collectionSettings.SerializerRegistry.GetSerializer<MockUserCollection>();

            var mockCollection = new Mock<IMongoCollection<MockUserCollection>> { DefaultValue = DefaultValue.Mock, CallBase = true };
            mockCollection.SetupGet(s => s.DocumentSerializer).Returns(documentSerializer);
            mockCollection.SetupGet(s => s.Settings).Returns(collectionSettings);
            mockCollection.SetupGet(x => x.CollectionNamespace).Returns(collectionNamespace);

            var _documents = new List<MockUserCollection>();
            Mock<IAsyncCursor<MockUserCollection>> mockCursor = new Mock<IAsyncCursor<MockUserCollection>>() { CallBase = true };
            mockCursor.Setup(_ => _.Current).Returns(_documents);
            mockCursor
                .SetupSequence(_ => _.MoveNext(It.IsAny<CancellationToken>()))
                .Returns(true)
                .Returns(false);

            mockCollection.Setup(x => 
                x.FindSync(It.IsAny<FilterDefinition<MockUserCollection>>(), It.IsAny<FindOptions<MockUserCollection>>(), It.IsAny<CancellationToken>()))
                    .Returns(mockCursor.Object);
            
            mockCollection.Setup(x => 
                x.FindAsync(It.IsAny<FilterDefinition<MockUserCollection>>(), It.IsAny<FindOptions<MockUserCollection>>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(mockCursor.Object);

            mockCollection.Setup(x =>
                x.Aggregate(It.IsAny<PipelineDefinition<MockUserCollection, MockUserCollection>>(), It.IsAny<AggregateOptions>(), It.IsAny<CancellationToken>()))
                    .Returns((PipelineDefinition<MockUserCollection, MockUserCollection> pipeline, AggregateOptions options, CancellationToken cancellationToken) =>
                    {
                        return mockCursor.Object;
                    });

            mockCollection.Setup(x =>
                x.AggregateAsync(It.IsAny<PipelineDefinition<MockUserCollection, MockUserCollection>>(), It.IsAny<AggregateOptions>(), It.IsAny<CancellationToken>()))
                   .ReturnsAsync((PipelineDefinition<MockUserCollection, MockUserCollection> pipeline, AggregateOptions options, CancellationToken cancellationToken) =>
                   {
                       return mockCursor.Object;
                   });

            mockCollection.Setup(x => 
                x.InsertMany(It.IsAny<IEnumerable<MockUserCollection>>(), It.IsAny<InsertManyOptions>(), It.IsAny<CancellationToken>()))
                    .Callback<IEnumerable<MockUserCollection>, InsertManyOptions, CancellationToken>((documents, options, cancellationToken) =>
                    {
                        _documents.AddRange(documents);
                    });

            mockCollection.Setup(x =>
                x.InsertOne(It.IsAny<MockUserCollection>(), It.IsAny<InsertOneOptions>(), It.IsAny<CancellationToken>()))
                   .Callback<MockUserCollection, InsertOneOptions, CancellationToken>((document, options, cancellationToken) =>
                   {
                       _documents.Add(document);
                   });

            mockCollection.Setup(x => 
                x.ReplaceOne(It.IsAny<FilterDefinition<MockUserCollection>>(), It.IsAny<MockUserCollection>(), It.IsAny<ReplaceOptions>(), It.IsAny<CancellationToken>()))
                    .Callback<FilterDefinition<MockUserCollection>, MockUserCollection, ReplaceOptions, CancellationToken>((definition, document, options, cancellationToken) => {
                        Expression<Func<MockUserCollection, bool>> expression = ((dynamic)definition).Expression;
                        var record = _documents.FirstOrDefault(expression.Compile());
                        _documents.Remove(record);
                        _documents.Add(document);
                    });

            mockCollection.Setup(x => 
                x.DeleteOne(It.IsAny<FilterDefinition<MockUserCollection>>(), It.IsAny<CancellationToken>()))
                   .Callback<FilterDefinition<MockUserCollection>, CancellationToken>((definition, cancellationToken) =>
                   {
                       Expression<Func<MockUserCollection, bool>> expression = ((dynamic)definition).Expression;
                       var record = _documents.FirstOrDefault(expression.Compile());
                       _documents.Remove(record);
                   });

            mockCollection.Setup(x => 
                x.CountDocuments(It.IsAny<FilterDefinition<MockUserCollection>>(), It.IsAny<CountOptions>(), It.IsAny<CancellationToken>()))
                    .Returns<FilterDefinition<MockUserCollection>, CountOptions, CancellationToken>((definition, options, cancellationToken) =>
                    {
                        if (FilterDefinition<MockUserCollection>.Empty == definition)
                        {
                            return _documents.Count;
                        }

                        Expression<Func<MockUserCollection, bool>> expression = ((dynamic)definition).Expression;
                        return _documents.Count(expression.Compile());
                    });

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
            services.AddSingleton(mockConfig.Object);

            services.AddSingleton(mockContext.Object);
            var serviceProvider = services.BuildServiceProvider();

            DbContextBinder<MockDbContext>.Bind(mockContext.Object, serviceProvider);
            return mockContext.Object;
        }
    }
}
