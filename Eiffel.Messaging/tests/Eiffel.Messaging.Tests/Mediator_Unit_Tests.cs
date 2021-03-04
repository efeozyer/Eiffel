using Eiffel.Messaging.Abstractions;
using Eiffel.Messaging.Core;
using Eiffel.Messaging.Core.Exceptions;
using Eiffel.Messaging.Tests.Mocks.Events;
using Eiffel.Messaging.Tests.Mocks.Handlers;
using Eiffel.Messaging.Tests.Mocks.Messages;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Eiffel.Messaging.Tests
{
    public class Mediator_Unit_Tests
    {
        private readonly IServiceCollection _services;

        public Mediator_Unit_Tests()
        {
            _services = new ServiceCollection();
            _services.AddSingleton<IMediator, Mediator>(serviceProvider =>
            {
                var options = new MessagingMiddlewareOptions();
                return new Mediator(serviceProvider, options);
            });
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
            await mediator.PublishAsync(mockEvent, default);

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
            Task task() => mediator.DispatchAsync(command);

            // Assert
            await Assert.ThrowsAsync<HandlerCoultNotBeResolvedException>(task);
        }

        [Fact]
        public async Task Should_Dispatch_Query_To_Handler()
        {
            // Arrange
            var mockQuery = new MockQuery(0, 5);

            var handlerMock = new Mock<MockQueryHandler>();
            handlerMock.Setup(x => x.HandleAsync(It.IsAny<MockQuery>(), It.IsAny<CancellationToken>()));

            _services.AddSingleton(typeof(IQueryHandler<MockQuery, MockQueryResult>), handlerMock.Object);

            // Act
            await mediator.DispatchAsync<MockQueryResult>(mockQuery, default);

            // Assert
            handlerMock.Verify(x => x.HandleAsync(It.IsAny<MockQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Should_Throw_Exception_When_QueryHandler_Not_Registered()
        {
            // Arrange
            var query = new MockUnknownQuery();

            // Act
            Task task() => mediator.DispatchAsync(query);

            // Assert
            await Assert.ThrowsAsync<HandlerCoultNotBeResolvedException>(task);
        }

        private IMediator mediator =>
            _services.BuildServiceProvider().GetRequiredService<IMediator>();
    }
}
