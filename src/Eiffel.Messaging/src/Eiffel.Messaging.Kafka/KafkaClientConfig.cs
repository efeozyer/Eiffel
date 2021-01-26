using Confluent.Kafka;

namespace Eiffel.Messaging.Providers.Kafka
{
    public class KafkaClientConfig
    {
        public ConsumerConfig ConsumerConfig { get; }
        public ProducerConfig ProducerConfig { get; }
    }
}
