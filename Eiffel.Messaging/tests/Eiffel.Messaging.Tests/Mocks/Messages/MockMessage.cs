using Eiffel.Messaging.Abstractions;
using Eiffel.Messaging.Attributes;

namespace Eiffel.Messaging.Tests.Mocks.Messages
{
    [TopicName("test-topic")]
    public class MockMessage : Message
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

    public class MockUnknownMessage : Message
    {

    }

    public class MockValidatableMessage : Message, IValidatable
    {
        public void Validate()
        {
            throw new System.NotImplementedException();
        }
    }
}
