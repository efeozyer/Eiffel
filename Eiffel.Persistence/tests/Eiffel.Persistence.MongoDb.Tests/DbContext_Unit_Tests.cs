using Eiffel.Persistence.MongoDb.Tests.Mocks;
using Eiffel.Persistence.Shared.MockBuilders;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Moq;
using System.Collections.Generic;
using System.Linq;
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

            var mockDatabase = new MockDatabaseBuilder<MockUserCollection>(mockCollection.Object).Build();
            var mockClient = new MockClientBuilder(mockDatabase.Object).Build();

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
