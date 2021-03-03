using Eiffel.Messaging.Abstractions;

namespace Eiffel.Messaging.InMemory
{
    public class InMemoryClientConfig : IMessageQueueClientConfig
    {
        public string Name => "InMemory";
        public bool IsEnabled { get; set; }
        public bool EnableConsoleLogging { get; set; }
    }
}
