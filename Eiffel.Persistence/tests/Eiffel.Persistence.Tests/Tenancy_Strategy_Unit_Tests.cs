using Autofac;
using Eiffel.Persistence.Abstractions;
using Eiffel.Persistence.Tenancy;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Xunit;

namespace Eiffel.Persistence.Tests
{
    public class Tenancy_Strategy_Unit_Tests
    {
        private readonly ContainerBuilder _containerBuilder;
        private readonly Mock<TenancyDbContext> _mockTenancyDbContext;
        private readonly Mock<IDefaultConnectionStringProvider> _mockDefaultConnectionStringProvider;

        public Tenancy_Strategy_Unit_Tests()
        {
            _containerBuilder = new ContainerBuilder();
            _mockTenancyDbContext = new Mock<TenancyDbContext>(new DbContextOptions<TenancyDbContext>());

            _mockDefaultConnectionStringProvider = new Mock<IDefaultConnectionStringProvider>();
            _mockDefaultConnectionStringProvider.Setup(x => x.Get()).Returns("connectionString");
        }

        [Fact]
        public void DatabasePerTenantStrategy_Should_Register_Context_When_Executed()
        {
            // Arrange
            var connectionString1 = "testConnectionString1";
            var tenantId1 = "tenantId1";

            var connectionString2 = "testConnectionString2";
            var tenantId2 = "tenantId2";

            var tenants = new List<TenantEntity>()
            {
                new TenantEntity
                {
                    Id = tenantId1,
                    ConnectionString = connectionString1,
                    CreatedBy = "user1",
                    ExpiresOn = DateTime.UtcNow.AddDays(90),
                    IsDeleted = false,
                    IsEnabled = true,
                    Name = "tenant1",
                },
                new TenantEntity
                {
                    Id = tenantId2,
                    ConnectionString = connectionString2,
                    CreatedBy = "user1",
                    ExpiresOn = DateTime.UtcNow.AddDays(90),
                    IsDeleted = false,
                    IsEnabled = true,
                    Name = "tenant2",
                },
            };

            FillContext(tenants);

            var strategy = new DatabasePerTenantStrategy<MockDbContext>(_containerBuilder, _mockTenancyDbContext.Object);

            // Act
            strategy.Execute();

            var container = _containerBuilder.Build();
            var tenant1Context = container.ResolveKeyed<MockDbContext>(tenantId1);
            var tenant1Metadata = container.ResolveKeyed<ITenantMetadata>("tenantId1");

            var tenant2Context = container.ResolveKeyed<MockDbContext>(tenantId2);
            var tenant2Metadata = container.ResolveKeyed<ITenantMetadata>("tenantId2");

            // Assert
            tenant1Context.Database.GetConnectionString().Should().Be(connectionString1);
            tenant2Context.Database.GetConnectionString().Should().Be(connectionString2);

            tenant1Metadata.Should().NotBeNull();
            tenant1Metadata.Id.Should().Be(tenantId1);

            tenant2Metadata.Should().NotBeNull();
            tenant2Metadata.Id.Should().Be(tenantId2);
        }

        [Fact]
        public void SchemaPerTenantStrategy_Should_Register_Context_When_Executed()
        {
            // Arrange
            var schemaName1 = "testSchema1";
            var tenantId1 = "tenantId1";

            var schemaName2 = "testSchema2";
            var tenantId2 = "tenantId2";

            var tenants = new List<TenantEntity>()
            {
                new TenantEntity
                {
                    Id = tenantId1,
                    SchemaName = schemaName1,
                    CreatedBy = "user1",
                    ExpiresOn = DateTime.UtcNow.AddDays(90),
                    IsDeleted = false,
                    IsEnabled = true,
                    Name = "tenant1"
                },
                new TenantEntity
                {
                    Id = tenantId2,
                    SchemaName = schemaName2,
                    CreatedBy = "user1",
                    ExpiresOn = DateTime.UtcNow.AddDays(90),
                    IsDeleted = false,
                    IsEnabled = true,
                    Name = "tenant2"
                },
            };

            FillContext(tenants);

            var strategy = new SchemaPerTenantStrategy<MockShemaContext>(_containerBuilder, _mockTenancyDbContext.Object, _mockDefaultConnectionStringProvider.Object);

            // Act
            strategy.Execute();

            var container = _containerBuilder.Build();

            var tenant1Context = container.ResolveKeyed<MockShemaContext>(tenantId1);
            var tenant1Metadata = container.ResolveKeyed<ITenantMetadata>(tenantId1);

            var tenant2Context = container.ResolveKeyed<MockShemaContext>(tenantId2);
            var tenant2Metadata = container.ResolveKeyed<ITenantMetadata>(tenantId2);

            // Assert
            tenant1Context.Should().NotBeNull();
            tenant1Metadata.Should().NotBeNull();
            tenant1Metadata.Id.Should().Be(tenantId1);
            tenant1Metadata.Value.Should().Be(schemaName1);

            tenant2Context.Should().NotBeNull();
            tenant2Metadata.Should().NotBeNull();
            tenant2Metadata.Id.Should().Be(tenantId2);
            tenant2Metadata.Value.Should().Be(schemaName2);

            _mockDefaultConnectionStringProvider.Verify(x => x.Get(), Times.Once);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void SchemaPerTenantStrategy_Should_Fail_When_ConnectionString_IsNullOrWhiteSpace(string connectionString)
        {
            // Arramge
            _mockDefaultConnectionStringProvider.Setup(x => x.Get()).Returns(connectionString);

            var strategy = new SchemaPerTenantStrategy<MockShemaContext>(_containerBuilder, _mockTenancyDbContext.Object, _mockDefaultConnectionStringProvider.Object);

            // Act
            Action execute = () => strategy.Execute();

            // Assert
            Assert.Throws<ArgumentNullException>(execute);
        }

        [Fact]
        public void SchemaPerTenantStrategy_Should_Fail_When_Schema_Property_NotDefined()
        {
            // Arramge
            var strategy = new SchemaPerTenantStrategy<MockDbContext>(_containerBuilder, _mockTenancyDbContext.Object, _mockDefaultConnectionStringProvider.Object);

            // Act
            Action execute = () => strategy.Execute();

            // Assert
            Assert.Throws<MissingMemberException>(execute);
        }

        private void FillContext(List<TenantEntity> tenants)
        {
            var dbSet = new Mock<DbSet<TenantEntity>>();

            var queryable = tenants.AsQueryable();

            dbSet.As<IQueryable<TenantEntity>>().Setup(x => x.Provider).Returns(queryable.Provider);
            dbSet.As<IQueryable<TenantEntity>>().Setup(x => x.Expression).Returns(queryable.Expression);
            dbSet.As<IQueryable<TenantEntity>>().Setup(x => x.ElementType).Returns(queryable.ElementType);
            dbSet.As<IQueryable<TenantEntity>>().Setup(x => x.GetEnumerator()).Returns(queryable.GetEnumerator());

            _mockTenancyDbContext.SetupGet(x => x.Tenants).Returns(dbSet.Object);
        }

        public class MockDbContext : DbContext 
        {
            public MockDbContext(DbContextOptions<MockDbContext> options) : base(options)
            {

            }
        }

        public class TestEntitiy
        {
            [Key]
            public int Id { get; set; }
        }

        public class MockShemaContext : DbContext
        {

            public virtual string Schema { get; set; }

            public DbSet<TestEntitiy> Entities { get; set; }

            public MockShemaContext(DbContextOptions<MockShemaContext> options) : base(options)
            {

            }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                if (!string.IsNullOrEmpty(Schema))
                {
                    modelBuilder.HasDefaultSchema(Schema);
                }
            }
        }
    }
}
