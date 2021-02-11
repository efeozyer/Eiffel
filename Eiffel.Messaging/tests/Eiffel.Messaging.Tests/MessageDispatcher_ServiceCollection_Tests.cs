using Eiffel.Messaging.Abstractions;
using Eiffel.Messaging.Core;
using Eiffel.Messaging.Tests.Mocks.Messages;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Xunit;

namespace Eiffel.Messaging.Tests
{
    public class MessageDispatcher_ServiceCollection_Tests
    {
        private readonly IServiceCollection _services;

        public MessageDispatcher_ServiceCollection_Tests()
        {
            _services = new ServiceCollection();
        }

        [Fact]
        public void Should_Register_Handlers()
        {
            // Arrange
            _services.AddMessageHandlers();
            var serviceProvider = _services.BuildServiceProvider();

            // Act
            var eventHandlers = serviceProvider.GetServices<IEventHandler<MockEvent>>();
            var commandHandler = serviceProvider.GetRequiredService<ICommandHandler<MockCommand>>();
            var queryHandler = serviceProvider.GetRequiredService<IQueryHandler<MockQuery, MockQueryResult>>();

            // Assert
            eventHandlers.Should().NotBeNullOrEmpty();
            eventHandlers.Count().Should().Be(2);
            commandHandler.Should().NotBeNull();
            queryHandler.Should().NotBeNull();
        }

        [Fact]
        public void Should_Register_Middlewares()
        {
            // Arrange
            _services.AddMessagingMiddlewares();
            var serviceProvider = _services.BuildServiceProvider();

            // Act
            var middlewares = serviceProvider.GetServices<IMessagingMiddleware>();

            // Assert
            middlewares.Should().NotBeNullOrEmpty();
            middlewares.Count().Should().Be(2);
        }

        [Fact]
        public void Should_Register_Message_Dispatcher()
        {
            // Arrange
            _services.AddMediator();
            var serviceProvider = _services.BuildServiceProvider();

            // Act
            var service = serviceProvider.GetService<IMediator>();

            // Assert
            service.Should().NotBeNull();
        }
    }
}
