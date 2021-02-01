using Eiffel.Messaging.Abstractions;
using Eiffel.Messaging.Core.Attributes;

namespace Eiffel.Samples.Contracts.Events
{
    [TopicName("eiffel-test")]
    public class ProductCreatedEvent : IEvent
    {
        public int ProductId { get; private set; }  
        public string ProductName { get; private set; }

        public ProductCreatedEvent()
        {

        }

        public ProductCreatedEvent(int productId, string productName)
        {
            ProductId = productId;
            ProductName = productName;
        }
    }
}
