using Eiffel.Persistence.Tenancy;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using Xunit;

namespace Eiffel.Persistence.Tests
{
    public class Request_Header_Identification_Tests
    {
        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private readonly RequestHeaderIdentificationStrategy _identificationStrategy;

        public Request_Header_Identification_Tests()
        {
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _identificationStrategy = new RequestHeaderIdentificationStrategy(_mockHttpContextAccessor.Object);
        }

        [Fact]
        public void Identification_Should_Fail_When_RequestHeaders_Not_Containst_TenantId()
        {
            // Arrange
            var defaultContext = new DefaultHttpContext();
            _mockHttpContextAccessor.SetupGet(x => x.HttpContext).Returns(defaultContext);

            // Act
            Func<string> identify = () => _identificationStrategy.Identify();

            // Assert
            Assert.Throws<TenantIdentificationFailedException>(identify);
        }
    }
}
