﻿using Eiffel.Messaging.Abstractions;
using Eiffel.Messaging.Attributes;

namespace Eiffel.Messaging.Tests.Mocks.Events
{
    [TopicName("mock-topic")]
    public class MockEvent : Event
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
