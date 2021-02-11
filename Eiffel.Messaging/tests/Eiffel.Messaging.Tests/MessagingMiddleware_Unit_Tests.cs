using Eiffel.Messaging.Abstractions;
using Eiffel.Messaging.Core;
using Eiffel.Messaging.Tests.Mocks.Exceptions;
using Eiffel.Messaging.Tests.Mocks.Handlers;
using Eiffel.Messaging.Tests.Mocks.Messages;
using Eiffel.Messaging.Tests.Mocks.Middlewares;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Eiffel.Messaging.Tests
{
    public class MessagingMiddleware_Unit_Tests
    {

        private readonly IServiceCollection _services;
        private readonly Mock<LoggingMiddleware> _loggingMiddleware;
        private readonly Mock<ValidationMiddleware> _validationMiddleware;

        public MessagingMiddleware_Unit_Tests()
        {
            _services = new ServiceCollection();
            _loggingMiddleware = new Mock<LoggingMiddleware>() { CallBase = true };
            _loggingMiddleware.Setup(x => x.InvokeAsync(It.IsAny<MockQuery>(), It.IsAny<CancellationToken>()));

            _validationMiddleware = new Mock<ValidationMiddleware>() { CallBase = true };
            _validationMiddleware.Setup(x => x.InvokeAsync(It.IsAny<MockQuery>(), It.IsAny<CancellationToken>()));

            _services.AddSingleton<IMediator, Mediator>(serviceProvider =>
            {
                var options = new MiddlewareOptions();
                options.Add(typeof(IMessagingMiddleware), _loggingMiddleware.Object);
                options.Add(typeof(IMessagingMiddleware), _validationMiddleware.Object);
                return new Mediator(serviceProvider, options);
            });
        }

        /// <summary>
        /// For debugging purposes set CallBase true
        /// </summary>
        [Fact]
        public async Task Should_Apply_Middlewares()
        {
            // Arrange
            var mockQuery = new MockQuery(0, 5);
            var mockCommand = new MockCommand();

            var mockQueryHandler = new Mock<MockQueryHandler>() { CallBase = true };
            mockQueryHandler.Setup(x => x.HandleAsync(It.IsAny<MockQuery>(), It.IsAny<CancellationToken>()));
            _services.AddSingleton(typeof(IQueryHandler<MockQuery, MockQueryResult>), mockQueryHandler.Object);

            var mockCommandHandler = new Mock<MockCommandHandler>() { CallBase = true };
            mockCommandHandler.Setup(x => x.HandleAsync(It.IsAny<MockCommand>(), It.IsAny<CancellationToken>()));
            _services.AddSingleton(typeof(ICommandHandler<MockCommand>), mockCommandHandler.Object);

            // Act
            var result = await mediator.DispatchAsync<MockQueryResult>(mockQuery, default);
            await mediator.DispatchAsync(mockCommand, default);

            // Assert
            mockQueryHandler.Verify(x => x.HandleAsync(It.IsAny<MockQuery>(), It.IsAny<CancellationToken>()), Times.Once);
            mockCommandHandler.Verify(x => x.HandleAsync(It.IsAny<MockCommand>(), It.IsAny<CancellationToken>()), Times.Once);

            _loggingMiddleware.Verify(x => x.InvokeAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
            _validationMiddleware.Verify(x => x.InvokeAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
            result.Items.Count.Should().Be(3);
        }

        /// <summary>
        /// For debugging purposes set CallBase true
        /// </summary>
        [Fact]
        public async Task Should_Handle_Middleware_Exceptions()
        {
            // Arrange
            var mockCommand = new MockInvalidCommand();

            var mockCommandHandler = new Mock<MockCommandHandler>() { CallBase = true };
            mockCommandHandler.Setup(x => x.HandleAsync(It.IsAny<MockInvalidCommand>(), It.IsAny<CancellationToken>()));
            _services.AddSingleton(typeof(ICommandHandler<MockInvalidCommand>), mockCommandHandler.Object);

            // Act
            Task task() => mediator.DispatchAsync(mockCommand);

            // Assert
            await Assert.ThrowsAsync<ValidationException>(task);
            _validationMiddleware.Verify(x => x.InvokeAsync(It.IsAny<MockInvalidCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        private IMediator mediator =>
           _services.BuildServiceProvider().GetRequiredService<IMediator>();
    }
}
