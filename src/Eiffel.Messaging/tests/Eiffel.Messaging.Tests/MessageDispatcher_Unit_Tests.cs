using Eiffel.Messaging.Abstractions;
using Eiffel.Messaging.Abstractions.Event;
using Eiffel.Messaging.Abstractions.Query;
using Eiffel.Messaging.Core;
using Eiffel.Messaging.Core.Exceptions;
using Eiffel.Messaging.Tests.Mocks.Handlers;
using Eiffel.Messaging.Tests.Mocks.Messages;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Eiffel.Messaging.Tests
{
    public class MessageDispatcher_Unit_Tests
    {
        private readonly IServiceCollection _services;

        public MessageDispatcher_Unit_Tests()
        {
            _services = new ServiceCollection();
            _services.AddMessageHandlers();
            _services.AddMessageDispatcher();
        }

        [Fact]
        public async Task Should_Dispatch_Event_To_EventHandlers()
        {
            // Arrange
            var mockEvent = new MockEvent();

            var handlerMock1 = new Mock<MockEventHandler1>();
            handlerMock1.Setup(x => x.HandleAsync(It.IsAny<MockEvent>(), It.IsAny<CancellationToken>()));
            _services.AddSingleton(typeof(IEventHandler<MockEvent>), handlerMock1.Object);

            var handlerMock2 = new Mock<MockEventHandler2>();
            handlerMock2.Setup(x => x.HandleAsync(It.IsAny<MockEvent>(), It.IsAny<CancellationToken>()));
            _services.AddSingleton(typeof(IEventHandler<MockEvent>), handlerMock2.Object);

            // Act
            await dispatcher.PublishAsync(mockEvent, default);

            // Assert
            handlerMock1.Verify(x => x.HandleAsync(It.IsAny<MockEvent>(), It.IsAny<CancellationToken>()), Times.Once);
            handlerMock2.Verify(x => x.HandleAsync(It.IsAny<MockEvent>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Should_Throw_Exception_When_CommandHandler_Not_Registered()
        {
            // Arrange
            var command = new MockUnknownCommand();

            // Act
            Task task() => dispatcher.DispatchAsync(command);

            // Assert
            await Assert.ThrowsAsync<HandlerCoultNotBeResolvedException>(task);
        }

        [Fact]
        public async Task Should_Dispatch_Query_To_Handler()
        {
            // Arrange
            var mockQuery = new MockQuery();

            var handlerMock = new Mock<MockQueryHandler>();
            handlerMock.Setup(x => x.HandleAsync(It.IsAny<MockQuery>(), It.IsAny<CancellationToken>()));

            _services.AddSingleton(typeof(IQueryHandler<MockQuery, MockQueryResult>), handlerMock.Object);

            // Act
            await dispatcher.DispatchAsync(mockQuery, default);

            // Assert
            handlerMock.Verify(x => x.HandleAsync(It.IsAny<MockQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Should_Throw_Exception_When_QueryHandler_Not_Registered()
        {
            // Arrange
            var query = new MockUnknownQuery();

            // Act
            Task task() => dispatcher.DispatchAsync(query);

            // Assert
            await Assert.ThrowsAsync<HandlerCoultNotBeResolvedException>(task);
        }

        private IMessageDispatcher dispatcher =>
            _services.BuildServiceProvider().GetRequiredService<IMessageDispatcher>();
    }
}
