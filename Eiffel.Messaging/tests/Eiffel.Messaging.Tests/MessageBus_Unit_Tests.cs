using Eiffel.Messaging.Abstractions;
using Eiffel.Messaging.Extensions;
using Eiffel.Messaging.InMemory;
using Eiffel.Messaging.Kafka;
using Eiffel.Messaging.Tests.Mocks;
using Eiffel.Messaging.Tests.Mocks.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Eiffel.Messaging.Tests
{
    public class MessageBus_Unit_Tests
    {
        private readonly IConfiguration _configuration;

        public MessageBus_Unit_Tests()
        {
            _configuration = Config.RootConfig();
        }

        [Fact]
        public async Task MessageBus_Should_Publish_Message_InMemoryClient()
        {
            // Arrange
            var mockMessage = new MockMessage("test");

            var mockClient = new Mock<InMemoryClient>(new InMemoryClientConfig());
            mockClient.Setup(x => x.Produce(It.IsAny<string>(), It.IsAny<MockMessage>()));
            mockClient.Setup(x => x.ProduceAsync(It.IsAny<string>(), It.IsAny<MockMessage>(), It.IsAny<CancellationToken>()));

            var mockMediator = new Mock<IMediator>();
            var messageBus = new MessageBus(mockClient.Object, mockMediator.Object, new MessagingMiddlewareOptions());

            // Act
            messageBus.Send(mockMessage);
            await messageBus.SendAsync(mockMessage, default);

            // Assert
            mockClient.Verify(x => x.Produce(It.IsAny<string>(), It.IsAny<MockMessage>()), Times.Once);
            mockClient.Verify(x => x.ProduceAsync(It.IsAny<string>(), It.IsAny<MockMessage>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task MessageBus_Should_Consume_Message_InMemoryClient()
        {
            // Arrange
            var mockClient = new Mock<InMemoryClient>(new InMemoryClientConfig());

            mockClient.Setup(x => x.Consume(It.IsAny<string>(), It.IsAny<Action<MockMessage>>()));
            mockClient.Setup(x => x.ConsumeAsync(It.IsAny<string>(), It.IsAny<Action<MockMessage>>(), It.IsAny<CancellationToken>()));

            var mockMediator = new Mock<IMediator>();
            var messageBus = new MessageBus(mockClient.Object, mockMediator.Object, new MessagingMiddlewareOptions());

            // Act
            messageBus.Subscribe<MockMessage>();
            await messageBus.SubscribeAsync<MockMessage>(default);

            // Assert
            mockClient.Verify(x => x.Consume(It.IsAny<string>(), It.IsAny<Action<MockMessage>>()), Times.Once);
            mockClient.Verify(x => x.ConsumeAsync(It.IsAny<string>(), It.IsAny<Action<MockMessage>>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task MessageBus_Should_Publish_Message_KafkaClient()
        {
            // Arrange
            var mockMessage = new MockMessage("test");
            var mockLogger = new Mock<ILogger<KafkaClient>>();
            var clientConfig = new KafkaClientConfig();
            clientConfig.Bind(_configuration);

            var mockClient = new Mock<KafkaClient>(mockLogger.Object, clientConfig);
            mockClient.Setup(x => x.Produce(It.IsAny<string>(), It.IsAny<MockMessage>()));
            mockClient.Setup(x => x.ProduceAsync(It.IsAny<string>(), It.IsAny<MockMessage>(), It.IsAny<CancellationToken>()));

            var mockMediator = new Mock<IMediator>();
            var messageBus = new MessageBus(mockClient.Object, mockMediator.Object, new MessagingMiddlewareOptions());

            // Act
            messageBus.Send(mockMessage);
            await messageBus.SendAsync(mockMessage, default);

            // Assert
            mockClient.Verify(x => x.Produce(It.IsAny<string>(), It.IsAny<MockMessage>()), Times.Once);
            mockClient.Verify(x => x.ProduceAsync(It.IsAny<string>(), It.IsAny<MockMessage>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task MessageBus_Should_Consume_Message_KafkaClient()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<KafkaClient>>();
            var clientConfig = new KafkaClientConfig();
            clientConfig.Bind(_configuration);
            var mockClient = new Mock<KafkaClient>(mockLogger.Object, clientConfig);

            mockClient.Setup(x => x.Consume(It.IsAny<string>(), It.IsAny<Action<MockMessage>>()));
            mockClient.Setup(x => x.ConsumeAsync(It.IsAny<string>(), It.IsAny<Action<MockMessage>>(), It.IsAny<CancellationToken>()));

            var mockMediator = new Mock<IMediator>();
            var messageBus = new MessageBus(mockClient.Object, mockMediator.Object, new MessagingMiddlewareOptions());

            // Act
            messageBus.Subscribe<MockMessage>();
            await messageBus.SubscribeAsync<MockMessage>(default);

            // Assert
            mockClient.Verify(x => x.Consume(It.IsAny<string>(), It.IsAny<Action<MockMessage>>()), Times.Once);
            mockClient.Verify(x => x.ConsumeAsync(It.IsAny<string>(), It.IsAny<Action<MockMessage>>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public void MessageBus_Should_Unsubscribe()
        {
            // Arrange
            var mockClient = new Mock<InMemoryClient>(new InMemoryClientConfig());
            mockClient.Setup(x => x.Unsubscribe());

            var mockMediator = new Mock<IMediator>();
            var messageBus = new MessageBus(mockClient.Object, mockMediator.Object, new MessagingMiddlewareOptions());

            // Act
            messageBus.Unsubscribe();

            // Assert
            mockClient.Verify(x => x.Unsubscribe(), Times.Once);
        }
    }
}
