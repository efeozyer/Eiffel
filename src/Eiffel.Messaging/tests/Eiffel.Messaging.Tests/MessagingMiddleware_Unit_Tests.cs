using Eiffel.Messaging.Abstractions;
using Eiffel.Messaging.Abstractions.Command;
using Eiffel.Messaging.Abstractions.Query;
using Eiffel.Messaging.Core;
using Eiffel.Messaging.Tests.Mocks.Exceptions;
using Eiffel.Messaging.Tests.Mocks.Handlers;
using Eiffel.Messaging.Tests.Mocks.Messages;
using Eiffel.Messaging.Tests.Mocks.Middlewares;
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

        public MessagingMiddleware_Unit_Tests()
        {
            _services = new ServiceCollection();
            _services.AddMessageHandlers();
            _services.AddMediator();
        }

        /// <summary>
        /// For debugging purposes set CallBase true
        /// </summary>
        [Fact]
        public async Task Should_Dispatch_Via_Middlewares()
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

            var mockLoggingMiddleware = new Mock<LoggingMiddleware>() { CallBase = true };
            mockLoggingMiddleware.Setup(x => x.Invoke(It.IsAny<MockQuery>(), It.IsAny<MessagingDelegate<MockQueryResult>>()));
            _services.AddSingleton(typeof(IMessagingMiddleware), mockLoggingMiddleware.Object);

            var mockValidationMiddleware = new Mock<ValidationMiddleware>() { CallBase = true };
            mockValidationMiddleware.Setup(x => x.Invoke(It.IsAny<MockQuery>(), It.IsAny<MessagingDelegate<MockQueryResult>>()));
            _services.AddSingleton(typeof(IMessagingMiddleware), mockValidationMiddleware.Object);

            // Act
            await mediator.DispatchAsync(mockQuery, default);
            await mediator.DispatchAsync(mockCommand, default);

            // Assert
            mockQueryHandler.Verify(x => x.HandleAsync(It.IsAny<MockQuery>(), It.IsAny<CancellationToken>()), Times.Once);
            mockCommandHandler.Verify(x => x.HandleAsync(It.IsAny<MockCommand>(), It.IsAny<CancellationToken>()), Times.Once);

            mockLoggingMiddleware.Verify(x => x.Invoke(It.IsAny<MockQuery>(), It.IsAny<MessagingDelegate<MockQueryResult>>()), Times.Once);
            mockLoggingMiddleware.Verify(x => x.Invoke(It.IsAny<MockCommand>(), It.IsAny<MessagingDelegate>()), Times.Once);

            mockValidationMiddleware.Verify(x => x.Invoke(It.IsAny<MockQuery>(), It.IsAny<MessagingDelegate<MockQueryResult>>()), Times.Once);
            mockValidationMiddleware.Verify(x => x.Invoke(It.IsAny<MockCommand>(), It.IsAny<MessagingDelegate>()), Times.Once);
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
        }

        private IMediator mediator =>
           _services.BuildServiceProvider().GetRequiredService<IMediator>();
    }
}
