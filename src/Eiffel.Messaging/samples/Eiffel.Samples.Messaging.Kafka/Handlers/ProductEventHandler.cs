using Eiffel.Messaging.Abstractions.Event;
using Eiffel.Samples.Contracts.Events;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Eiffel.Samples.Messaging.Kafka.Handlers
{
    public class ProductEventHandler : IEventHandler<ProductCreatedEvent>
    {
        public async Task HandleAsync(ProductCreatedEvent @event, CancellationToken cancellationToken)
        {
            await Console.Out.WriteLineAsync($"Product created (self) {@event.ProductId}-{@event.ProductName}");
        }
    }

    public class WarehouseEventHandler : IEventHandler<ProductCreatedEvent>
    {
        public async Task HandleAsync(ProductCreatedEvent @event, CancellationToken cancellationToken)
        {
            await Console.Out.WriteLineAsync($"Product created (warehouse) {@event.ProductId}-{@event.ProductName}");
        }
    }   
}
