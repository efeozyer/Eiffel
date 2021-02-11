using Eiffel.Messaging.Abstractions;
using Microsoft.Extensions.Configuration;

namespace Eiffel.Messaging.InMemory
{
    public class InMemoryClientConfig : IMessageQueueClientConfig
    {
        public bool IsEnabled { get; set; }
        public bool EnableConsoleLogging { get; set; }

        public void Bind(IConfiguration configuration)
        {
            configuration.GetSection("Messaging:InMemory").Bind(this);
        }
    }
}
