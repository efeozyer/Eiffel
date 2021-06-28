using Autofac;
using Autofac.Extras.Moq;
using Eiffel.Persistence.Abstractions;
using Eiffel.Persistence.Tenancy;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Eiffel.Persistence.Tests
{
    public class MultiTenantcy_Middleware_Tests
    {
        private readonly Mock<TenancyDbContext> _mockTenancyDbContext;
        private readonly Mock<ITenantIdentificationStrategy<string>> _mockIdentificationStrategy;

        private readonly Mock<RequestDelegate> _mockRequestDeletegate;

        private readonly MultiTenancyMiddleware _tenancyMiddleware;

        private readonly IContainer container;

        public MultiTenantcy_Middleware_Tests()
        {
            _mockTenancyDbContext = new Mock<TenancyDbContext>(new DbContextOptions<TenancyDbContext>());
            _mockIdentificationStrategy = new Mock<ITenantIdentificationStrategy<string>>();

            _mockRequestDeletegate = new Mock<RequestDelegate>();

            var mock = AutoMock.GetLoose(x => x.RegisterMock(_mockTenancyDbContext));
            _tenancyMiddleware = new MultiTenancyMiddleware(_mockRequestDeletegate.Object, _mockIdentificationStrategy.Object, mock.Container.BeginLifetimeScope());
            
            container = mock.Container;

            Init();
        }

        [Fact]
        public async Task Identification_Should_Throws_Exception_When_TenancyId_Null()
        {
            // Arrange
            var defaultContext = new DefaultHttpContext();
            _mockIdentificationStrategy.Setup(x => x.Identify()).Returns<string>(null);

            // Act
            Func<Task> invoke = () => _tenancyMiddleware.InvokeAsync(defaultContext);

            // Assert
            await Assert.ThrowsAsync<TenantIdentificationFailedException>(invoke);
            _mockIdentificationStrategy.Verify(x => x.Identify(), Times.Once);
        }

        [Fact]
        public async Task Identification_Should_Throws_Exception_When_TenancyId_Invalid()
        {
            // Arrange
            var defaultContext = new DefaultHttpContext();
            _mockIdentificationStrategy.Setup(x => x.Identify()).Returns("invalidId");

            // Act
            Func<Task> invoke = () => _tenancyMiddleware.InvokeAsync(defaultContext);

            // Assert
            await Assert.ThrowsAsync<TenantNotFoundException>(invoke);
            _mockIdentificationStrategy.Verify(x => x.Identify(), Times.Once);
        }

        [Fact]
        public async Task Identification_Should_Throws_Exception_When_Tenant_Deleted()
        {
            // Arrange
            var defaultContext = new DefaultHttpContext();
            _mockIdentificationStrategy.Setup(x => x.Identify()).Returns("tenantId2");

            // Act
            Func<Task> invoke = () => _tenancyMiddleware.InvokeAsync(defaultContext);

            // Assert
            await Assert.ThrowsAsync<TenantIdentificationFailedException>(invoke);
            _mockIdentificationStrategy.Verify(x => x.Identify(), Times.Once);
        }

        [Fact]
        public async Task Identification_Should_Throws_Exception_When_Tenant_Disabled()
        {
            // Arrange
            var defaultContext = new DefaultHttpContext();
            _mockIdentificationStrategy.Setup(x => x.Identify()).Returns("tenantId3");

            // Act
            Func<Task> invoke = () => _tenancyMiddleware.InvokeAsync(defaultContext);

            // Assert
            await Assert.ThrowsAsync<TenantIdentificationFailedException>(invoke);
            _mockIdentificationStrategy.Verify(x => x.Identify(), Times.Once);
        }

        [Fact]
        public async Task Identification_Should_Throws_Exception_When_Tenant_Expired()
        {
            // Arrange
            var defaultContext = new DefaultHttpContext();
            _mockIdentificationStrategy.Setup(x => x.Identify()).Returns("tenantId4");

            // Act
            Func<Task> invoke = () => _tenancyMiddleware.InvokeAsync(defaultContext);

            // Assert
            await Assert.ThrowsAsync<TenantRegistrationExpiredException>(invoke);
            _mockIdentificationStrategy.Verify(x => x.Identify(), Times.Once);
        }

        [Fact]
        public async Task Should_Identify_Tenant()
        {
            // Arrange
            var tenantId = "tenantId1";
            var defaultContext = new DefaultHttpContext();
            _mockIdentificationStrategy.Setup(x => x.Identify()).Returns(tenantId);

            // Act
            await _tenancyMiddleware.InvokeAsync(defaultContext);

            // Assert
            _mockIdentificationStrategy.Verify(x => x.Identify(), Times.Once);
            _mockRequestDeletegate.Verify(x => x.Invoke(It.IsAny<HttpContext>()), Times.Once);
        }

        private void Init()
        {
            var tenants = new List<TenantEntity>()
            {
                new TenantEntity
                {
                    Id = "tenantId1",
                    ConnectionString = "testConnectionString",
                    CreatedBy = "user1",
                    ExpiresOn = DateTime.UtcNow.AddDays(90),
                    IsDeleted = false,
                    IsEnabled = true,
                    Name = "tenant1",
                },
                new TenantEntity
                {
                    Id = "tenantId2",
                    ConnectionString = "testConnectionString",
                    CreatedBy = "user1",
                    ExpiresOn = DateTime.UtcNow.AddDays(90),
                    IsDeleted = true,
                    IsEnabled = true,
                    Name = "tenant2",
                },
                new TenantEntity
                {
                    Id = "tenantId3",
                    ConnectionString = "testConnectionString",
                    CreatedBy = "user1",
                    ExpiresOn = DateTime.UtcNow.AddDays(90),
                    IsDeleted = false,
                    IsEnabled = false,
                    Name = "tenant3",
                },
                new TenantEntity
                {
                    Id = "tenantId4",
                    ConnectionString = "testConnectionString",
                    CreatedBy = "user1",
                    ExpiresOn = DateTime.UtcNow.AddDays(-1),
                    IsDeleted = false,
                    IsEnabled = true,
                    Name = "tenant4",
                }
            }.AsQueryable();

            var dbSet = new Mock<DbSet<TenantEntity>>();

            dbSet.As<IQueryable<TenantEntity>>().Setup(x => x.Provider).Returns(tenants.Provider);
            dbSet.As<IQueryable<TenantEntity>>().Setup(x => x.Expression).Returns(tenants.Expression);
            dbSet.As<IQueryable<TenantEntity>>().Setup(x => x.ElementType).Returns(tenants.ElementType);
            dbSet.As<IQueryable<TenantEntity>>().Setup(x => x.GetEnumerator()).Returns(tenants.GetEnumerator());

            _mockTenancyDbContext.SetupGet(x => x.Tenants).Returns(dbSet.Object);
        }
    }
}
