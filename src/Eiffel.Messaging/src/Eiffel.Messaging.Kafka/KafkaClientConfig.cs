using Confluent.Kafka;

namespace Eiffel.Messaging.Kafka
{
    public class KafkaClientConfig
    {
        public ConsumerConfig ConsumerConfig { get; }
        public ProducerConfig ProducerConfig { get; }
    }
}
