using Eiffel.Messaging.Core;

namespace Eiffel.Messaging.Kafka
{
    public static class ServiceCollectionExtensions
    {
        public static void UseKafka(this MessageQueueOptions<KafkaClientConfig> options, KafkaClientConfig clientConfig)
        {
            options.ClientConfig = clientConfig;
        }
    }
}
