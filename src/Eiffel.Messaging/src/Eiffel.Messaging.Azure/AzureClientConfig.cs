using Microsoft.Azure.ServiceBus;

namespace Eiffel.Messaging.Azure
{
    public class AzureClientConfig
    {
        public string QueueName { get; set; }
        public string ConnectionString { get; set; }
        public ReceiveMode ReceiveMode { get; set; }
    }
}
