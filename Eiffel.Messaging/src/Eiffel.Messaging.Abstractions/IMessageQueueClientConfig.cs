using Microsoft.Extensions.Configuration;

namespace Eiffel.Messaging.Abstractions
{
    public interface IMessageQueueClientConfig
    {
        bool IsEnabled { get; set; }
        bool EnableConsoleLogging { get; set; }
    }
}
