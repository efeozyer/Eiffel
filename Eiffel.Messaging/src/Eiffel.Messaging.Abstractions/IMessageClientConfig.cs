using Microsoft.Extensions.Configuration;

namespace Eiffel.Messaging.Abstractions
{
    public interface IMessageClientConfig
    {
        bool IsEnabled { get; set; }
        bool EnableConsoleLogging { get; set; }
    }
}
