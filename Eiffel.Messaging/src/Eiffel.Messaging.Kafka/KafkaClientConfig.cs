using Confluent.Kafka;
using Eiffel.Messaging.Abstractions;

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
    }
}
