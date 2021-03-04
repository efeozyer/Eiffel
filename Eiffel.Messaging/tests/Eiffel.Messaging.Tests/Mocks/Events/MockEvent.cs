using Eiffel.Messaging.Abstractions;
using Eiffel.Messaging.Core.Attributes;

namespace Eiffel.Messaging.Tests.Mocks.Events
{
    [TopicName("mock-topic")]
    public class MockEvent : IEvent
    {
        public string Name { get; private set; }

        public MockEvent()
        {

        }

        public MockEvent(string name)
        {
            Name = name;
        }
    }
}
