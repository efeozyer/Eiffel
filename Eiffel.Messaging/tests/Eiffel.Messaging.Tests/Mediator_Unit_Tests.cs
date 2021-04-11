using Eiffel.Messaging.Abstractions;
using Eiffel.Messaging.Exceptions;
using Eiffel.Messaging.Tests.Mocks.Events;
using Eiffel.Messaging.Tests.Mocks.Handlers;
using Eiffel.Messaging.Tests.Mocks.Messages;
using Eiffel.Messaging.Tests.Mocks.Pipelines;
using FluentAssertions;
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
        private IMediator mediator =>
            _services.BuildServiceProvider().GetRequiredService<IMediator>();

        public Mediator_Unit_Tests()
        {
            _services = new ServiceCollection();
            _services.AddSingleton<IMediator, Mediator>(serviceProvider =>
            {
                return new Mediator(serviceProvider);
            });
        }

        [Fact]
        public async Task Dispatch_Should_Success_When_EventHandler_Registered()
        {
            // Arrange
            var mockEvent = new MockEvent();

            var handlerMock1 = new Mock<MockEventHandler1>();
            handlerMock1.Setup(x => x.HandleAsync(It.IsAny<MockEvent>(), It.IsAny<CancellationToken>()));
            _services.AddSingleton(typeof(EventHandler<MockEvent>), handlerMock1.Object);

            var handlerMock2 = new Mock<MockEventHandler2>();
            handlerMock2.Setup(x => x.HandleAsync(It.IsAny<MockEvent>(), It.IsAny<CancellationToken>()));
            _services.AddSingleton(typeof(EventHandler<MockEvent>), handlerMock2.Object);

            // Act
            await mediator.PublishAsync(mockEvent, default);

            // Assert
            handlerMock1.Verify(x => x.HandleAsync(It.IsAny<MockEvent>(), It.IsAny<CancellationToken>()), Times.Once);
            handlerMock2.Verify(x => x.HandleAsync(It.IsAny<MockEvent>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Dispatch_Should_Throw_Exception_When_EventHandler_Not_Registered()
        {
            // Arrange
            var mockEvent = new MockEvent();

            var handlerMock1 = new Mock<MockEventHandler1>();
            handlerMock1.Setup(x => x.HandleAsync(It.IsAny<MockEvent>(), It.IsAny<CancellationToken>()));

            // Act
            Task task() => mediator.PublishAsync(mockEvent, default);

            // Assert
            await Assert.ThrowsAsync<HandlerCouldNotBeResolvedException>(task);
        }

        [Fact]
        public async Task Dispatch_Should_Success_When_QueryHandler_Registered()
        {
            // Arrange
            var mockQuery = new MockQuery(0, 5);

            var handlerMock = new Mock<MockQueryHandler>() { CallBase = true };
            handlerMock.Setup(x => x.HandleAsync(It.IsAny<MockQuery>(), It.IsAny<CancellationToken>()));

            _services.AddSingleton(typeof(QueryHandler<MockQuery, MockQueryResult>), handlerMock.Object);

            // Act
            await mediator.RequestAsync(mockQuery, default);

            // Assert
            handlerMock.Verify(x => x.HandleAsync(It.IsAny<MockQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Dispatch_Should_Throw_Exception_When_QueryHandler_Not_Registered()
        {
            // Arrange
            var mockQuery = new MockUnknownQuery();

            // Act
            Task task() => mediator.RequestAsync(mockQuery, default);

            // Assert
            await Assert.ThrowsAsync<HandlerCouldNotBeResolvedException>(task);
        }

        [Fact]
        public async Task Dispatch_Should_Success_When_CommandHandler_Registered()
        {
            // Arrange
            var mockCommand = new MockCommand();

            var handlerMock = new Mock<MockCommandHandler>() { CallBase = true };
            handlerMock.Setup(x => x.HandleAsync(It.IsAny<MockCommand>(), It.IsAny<CancellationToken>()));

            _services.AddSingleton(typeof(CommandHandler<MockCommand>), handlerMock.Object);

            // Act
            await mediator.SendAsync<string>(mockCommand, default);

            // Assert
            handlerMock.Verify(x => x.HandleAsync(It.IsAny<MockCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Dispatch_Should_Throw_Exception_When_CommandHandler_Not_Registered()
        {
            // Arrange
            var mockCommand = new MockUnknownQuery();

            // Act
            Task task() => mediator.RequestAsync(mockCommand, default);

            // Assert
            await Assert.ThrowsAsync<HandlerCouldNotBeResolvedException>(task);
        }

        [Fact]
        public async Task Dispatch_Should_Success_When_MessageHandler_Registered()
        {
            // Arrange
            var mockMessage = new MockMessage();

            var handlerMock = new Mock<MockMessageHandler>() { CallBase = true };
            handlerMock.Setup(x => x.HandleAsync(It.IsAny<MockMessage>(), It.IsAny<CancellationToken>()));

            _services.AddSingleton(typeof(MessageHandler<MockMessage>), handlerMock.Object);

            // Act
            await mediator.DispatchAsync(mockMessage, default);

            // Assert
            handlerMock.Verify(x => x.HandleAsync(It.IsAny<MockMessage>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Dispatch_Should_Throw_Exception_When_MessageHandler_Not_Registered()
        {
            // Arrange
            var mockMessage = new MockUnknownMessage();

            // Act
            Task task() => mediator.DispatchAsync(mockMessage, default);

            // Assert
            await Assert.ThrowsAsync<HandlerCouldNotBeResolvedException>(task);
        }

        [Fact]
        public async Task Pipelines_Should_Execute_Correct_Order()
        {
            // Arrange
            string callOrder = string.Empty;
            var mockMessage = new MockMessage();

            var prePipelineMock = new Mock<MockValidationPipeline>() { CallBase = true };
            prePipelineMock.Setup(x => x.ProcessAsync(It.IsAny<MockMessage>(), It.IsAny<CancellationToken>()))
                .Callback(() =>
                {
                    callOrder += "A";
                });

            var handlerMock = new Mock<MockMessageHandler>() { CallBase = true };
            handlerMock.Setup(x => x.HandleAsync(It.IsAny<MockMessage>(), It.IsAny<CancellationToken>()))
                .Callback(() =>
                {
                    callOrder += "B";
                });

            var postPipelineMock = new Mock<MockAuditLoggingPipeline>() { CallBase = true };
            postPipelineMock.Setup(x => x.ProcessAsync(It.IsAny<MockMessage>(), It.IsAny<CancellationToken>()))
                .Callback(() =>
                {
                    callOrder += "C";
                });

            _services.AddSingleton(typeof(MessageHandler<MockMessage>), handlerMock.Object);
            _services.AddSingleton(typeof(IPipelinePreProcessor), prePipelineMock.Object);
            _services.AddSingleton(typeof(IPipelinePostProcessor), postPipelineMock.Object);

            // Act
            await mediator.DispatchAsync(mockMessage, default);

            // Assert
            prePipelineMock.Verify(x => x.ProcessAsync(It.IsAny<MockMessage>(), It.IsAny<CancellationToken>()), Times.Once);
            handlerMock.Verify(x => x.HandleAsync(It.IsAny<MockMessage>(), It.IsAny<CancellationToken>()), Times.Once);
            postPipelineMock.Verify(x => x.ProcessAsync(It.IsAny<MockMessage>(), It.IsAny<CancellationToken>()), Times.Once);
            callOrder.Should().Be("ABC");
        }

        [Fact]
        public async Task Dispatch_Should_Throw_Pipeline_Exception()
        {
            // Arrange
            var mockMessage = new MockValidatableMessage();

            var validationPipeline = new Mock<MockValidationPipeline>() { CallBase = true };
            validationPipeline.Setup(x => x.ProcessAsync(It.IsAny<MockValidatableMessage>(), It.IsAny<CancellationToken>()));

            var handlerMock = new Mock<MockValidatableMessageHandler>() { CallBase = true };
            handlerMock.Setup(x => x.HandleAsync(It.IsAny<MockValidatableMessage>(), It.IsAny<CancellationToken>()));

            _services.AddSingleton(typeof(MessageHandler<MockValidatableMessage>), handlerMock.Object);
            _services.AddSingleton(typeof(IPipelinePreProcessor), validationPipeline.Object);

            // Act
            Task task() => mediator.DispatchAsync(mockMessage, default);

            // Assert
            await Assert.ThrowsAsync<System.NotImplementedException>(task);
        }
    }
}
