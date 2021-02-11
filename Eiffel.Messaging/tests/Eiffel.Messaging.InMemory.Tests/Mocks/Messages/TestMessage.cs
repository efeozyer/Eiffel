using Eiffel.Messaging.Abstractions;

namespace Eiffel.Messaging.InMemory.Tests.Mocks.Messages
{
    public class TestMessage : IMessage
    {
        public string Content { get; set; }

        public TestMessage()
        {

        }

        public TestMessage(string content)
        {
            Content = content;
        }
    }
}
