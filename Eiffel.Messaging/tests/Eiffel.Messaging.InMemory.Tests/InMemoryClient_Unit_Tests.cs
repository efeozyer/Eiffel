using Eiffel.Messaging.Abstractions;
using Eiffel.Messaging.InMemory.Tests.Mocks.Messages;
using FluentAssertions;
using Xunit;

namespace Eiffel.Messaging.InMemory.Tests
{
    public class InMemoryClient_Unit_Tests
    {
        private readonly IMessageQueueClient _client;

        public InMemoryClient_Unit_Tests()
        {
            _client = new InMemoryClient(new InMemoryClientConfig());
        }

        [Fact]
        public void Client_Should_Produce_And_Consume_Message()
        {
            // Arrange
            var message = new TestMessage("test");
            TestMessage result = null;

            // Act 
            _client.Consume<TestMessage>("test-topic", (msg) =>
            {
                result = msg;
            });

            _client.Produce("test-topic", message);

            // Assert
            result.Should().NotBeNull();
            result.Content.Should().Be("test");
        }

        [Fact]
        public void Client_Should_Unsubscribe()
        {
            // Arrange
            var message = new TestMessage("test");
            TestMessage result = null;

            // Act 
            _client.Consume<TestMessage>("test-topic", (msg) =>
            {
                result = msg;
            });

            _client.Unsubscribe();
            _client.Produce("test-topic", message);

            // Assert
            result.Should().BeNull();
        }
    }
}
