using Confluent.Kafka;
using Eiffel.Messaging.Abstractions;
using Microsoft.Extensions.Configuration;

namespace Eiffel.Messaging.Kafka
{
    public class KafkaClientConfig : IMessageClientConfig
    {
        public bool IsEnabled { get; set; }
        public bool EnableConsoleLogging { get; set; }
        public ConsumerConfig ConsumerConfig { get; set; }
        public ProducerConfig ProducerConfig { get; set; }

        public KafkaClientConfig()
        {

        }

        public void Bind(IConfiguration configuration)
        {
            configuration.GetSection("Messaging:Kafka").Bind(this);
        }
    }
}
