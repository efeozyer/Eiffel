using Eiffel.Messaging.Abstractions;
using Eiffel.Messaging.Core.Attributes;

namespace Eiffel.Messaging.Tests.Mocks.Messages
{
    [TopicName("test-topic")]
    public class MockMessage : IMessage
    {
        public string Message { get; set; }

        public MockMessage(string message)
        {
            Message = message;
        }

        public MockMessage()
        {

        }
    }
}
