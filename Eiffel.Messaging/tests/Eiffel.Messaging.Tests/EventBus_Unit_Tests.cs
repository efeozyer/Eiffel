using Eiffel.Messaging.Abstractions;
using Eiffel.Messaging.Extensions;
using Eiffel.Messaging.InMemory;
using Eiffel.Messaging.Kafka;
using Eiffel.Messaging.Tests.Mocks;
using Eiffel.Messaging.Tests.Mocks.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Eiffel.Messaging.Tests
{
    public class EventBus_Unit_Tests
    {
        private readonly IConfiguration _configuration;

        public EventBus_Unit_Tests()
        {
            _configuration = Config.RootConfig();
        }

        [Fact]
        public async Task EventBus_Should_Publish_Event_InMemoryClient()
        {
            // Arrange
            var mockEvent = new MockEvent("test");

            var mockClient = new Mock<InMemoryClient>(new InMemoryClientConfig());
            mockClient.Setup(x => x.Produce(It.IsAny<string>(), It.IsAny<MockEvent>()));
            mockClient.Setup(x => x.ProduceAsync(It.IsAny<string>(), It.IsAny<MockEvent>(), It.IsAny<CancellationToken>()));

            var mockMediator = new Mock<IMediator>();
            var eventBus = new Core.EventBus(mockClient.Object, mockMediator.Object);

            // Act
            eventBus.Publish(mockEvent);
            await eventBus.PublishAsync(mockEvent, default);

            // Assert
            mockClient.Verify(x => x.Produce(It.IsAny<string>(), It.IsAny<MockEvent>()), Times.Once);
            mockClient.Verify(x => x.ProduceAsync(It.IsAny<string>(), It.IsAny<MockEvent>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task EventBus_Should_Consume_Event_InMemoryClient()
        {
            // Arrange
            var mockClient = new Mock<InMemoryClient>(new InMemoryClientConfig());

            mockClient.Setup(x => x.Consume(It.IsAny<string>(), It.IsAny<Action<MockEvent>>()));
            mockClient.Setup(x => x.ConsumeAsync(It.IsAny<string>(), It.IsAny<Action<MockEvent>>(), It.IsAny<CancellationToken>()));

            var mockMediator = new Mock<IMediator>();
            var eventBus = new Core.EventBus(mockClient.Object, mockMediator.Object);

            // Act
            eventBus.Subscribe<MockEvent>();
            await eventBus.SubscribeAsync<MockEvent>(default);

            // Assert
            mockClient.Verify(x => x.Consume(It.IsAny<string>(), It.IsAny<Action<MockEvent>>()), Times.Once);
            mockClient.Verify(x => x.ConsumeAsync(It.IsAny<string>(), It.IsAny<Action<MockEvent>>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task EventBus_Should_Publish_Event_KafkaClient()
        {
            // Arrange
            var mockEvent = new MockEvent("test");
            var mockLogger = new Mock<ILogger<KafkaClient>>();
            var clientConfig = new KafkaClientConfig();
            clientConfig.Bind(_configuration);

            var mockClient = new Mock<KafkaClient>(mockLogger.Object, clientConfig);
            mockClient.Setup(x => x.Produce(It.IsAny<string>(), It.IsAny<MockEvent>()));
            mockClient.Setup(x => x.ProduceAsync(It.IsAny<string>(), It.IsAny<MockEvent>(), It.IsAny<CancellationToken>()));

            var mockMediator = new Mock<IMediator>();
            var eventBus = new Core.EventBus(mockClient.Object, mockMediator.Object);

            // Act
            eventBus.Publish(mockEvent);
            await eventBus.PublishAsync(mockEvent, default);

            // Assert
            mockClient.Verify(x => x.Produce(It.IsAny<string>(), It.IsAny<MockEvent>()), Times.Once);
            mockClient.Verify(x => x.ProduceAsync(It.IsAny<string>(), It.IsAny<MockEvent>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task EventBus_Should_Consume_Event_KafkaClient()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<KafkaClient>>();
            var clientConfig = new KafkaClientConfig();
            clientConfig.Bind(_configuration);
            var mockClient = new Mock<KafkaClient>(mockLogger.Object, clientConfig);

            mockClient.Setup(x => x.Consume(It.IsAny<string>(), It.IsAny<Action<MockEvent>>()));
            mockClient.Setup(x => x.ConsumeAsync(It.IsAny<string>(), It.IsAny<Action<MockEvent>>(), It.IsAny<CancellationToken>()));

            var mockMediator = new Mock<IMediator>();
            var eventBus = new Core.EventBus(mockClient.Object, mockMediator.Object);

            // Act
            eventBus.Subscribe<MockEvent>();
            await eventBus.SubscribeAsync<MockEvent>(default);

            // Assert
            mockClient.Verify(x => x.Consume(It.IsAny<string>(), It.IsAny<Action<MockEvent>>()), Times.Once);
            mockClient.Verify(x => x.ConsumeAsync(It.IsAny<string>(), It.IsAny<Action<MockEvent>>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public void EventBus_Should_Unsubscribe()
        {
            // Arrange
            var mockClient = new Mock<InMemoryClient>(new InMemoryClientConfig());
            mockClient.Setup(x => x.Unsubscribe());

            var mockMediator = new Mock<IMediator>();
            var eventBus = new Core.EventBus(mockClient.Object, mockMediator.Object);

            // Act
            eventBus.Unsubscribe();

            // Assert
            mockClient.Verify(x => x.Unsubscribe(), Times.Once);
        }
    }
}
