using Eiffel.Messaging.Abstractions;
using Microsoft.Extensions.Configuration;

namespace Eiffel.Messaging.Extensions
{
    public static class ConfigurationExtensions
    {
        public static IMessageQueueClientConfig Bind(this IMessageQueueClientConfig config, IConfiguration configuration)
        {
            configuration.GetSection($"Messaging:{config.Name}").Bind(config);
            return config;
        }
    }
}
