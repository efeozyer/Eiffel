using Eiffel.Persistence.MongoDb.Tests.Mocks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using FluentAssertions;
using MongoDB.Driver;

namespace Eiffel.Persistence.MongoDb.Tests
{
    public class DbContext_ServiceCollection_Tests
    {
        private readonly IServiceCollection _services;

        public DbContext_ServiceCollection_Tests()
        {
            _services = new ServiceCollection();
        }

        [Fact]
        public void Should_Register_CollectionTypeConfigurations()
        {
            // Arrange
            _services.AddDocumentContext(new DbContextOptions<MockDbContext>(new MongoClientSettings())
            {
                Database = "mock-database"
            });
            var serviceProvider = _services.BuildServiceProvider();

            // Act
            var configuration = serviceProvider.GetService(typeof(ICollectionTypeConfiguration<MockUserCollection>));

            // Assert
            configuration.Should().NotBeNull();
        }
    }
}
