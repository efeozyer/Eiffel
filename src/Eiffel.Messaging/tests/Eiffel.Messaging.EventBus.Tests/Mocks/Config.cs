using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace Eiffel.Messaging.EventBus.Tests.Mocks
{
    public static class Config
    {
        public static IConfiguration RootConfig()
        {
            var configContent = new List<KeyValuePair<string, string>>()
            {
                {
                    new KeyValuePair<string, string>("Messaging:InMemory:IsEnabled", "true")
                },
                {
                    new KeyValuePair<string, string>("Messaging:InMemory:EnableConsoleLogging", "true")
                },
                {
                    new KeyValuePair<string, string>("Messaging:Kafka:ConsumerConfig:BootstrapServers", "localhost:9092")
                },
                {
                    new KeyValuePair<string, string>("Messaging:Kafka:ConsumerConfig:GroupId", "EventBusTest")
                },
                {
                    new KeyValuePair<string, string>("Messaging:Kafka:ProducerConfig:BootstrapServers", "localhost:9092")
                },
                {
                    new KeyValuePair<string, string>("Messaging:Kafka:ProducerConfig:ClientId", "192.168.1.34")
                },
            };
            return new ConfigurationBuilder().AddInMemoryCollection(configContent).Build();
        }
    }
}
