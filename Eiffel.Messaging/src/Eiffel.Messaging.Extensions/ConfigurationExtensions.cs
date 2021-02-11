using Eiffel.Messaging.Abstractions;
using Microsoft.Extensions.Configuration;

namespace Eiffel.Messaging.Extensions
{
    public static class ConfigurationExtensions
    {
        public static IMessageClientConfig Bind(this IMessageClientConfig config, IConfiguration configuration)
        {
            configuration.GetSection("Messaging:Kafka").Bind(config);
            return config;
        }
    }
}
