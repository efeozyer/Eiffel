using Eiffel.Samples.Contracts.Events;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Eiffel.Samples.Messaging.Kafka.Handlers
{
    public class ProductEventHandler : Eiffel.Messaging.Abstractions.EventHandler<ProductCreatedEvent>
    {
        public override async Task HandleAsync(ProductCreatedEvent @event, CancellationToken cancellationToken = default)
        {
            await Console.Out.WriteLineAsync($"Product created (self) {@event.ProductId}-{@event.ProductName}");
        }
    }

    public class WarehouseEventHandler : Eiffel.Messaging.Abstractions.EventHandler<ProductCreatedEvent>
    {
        public override async Task HandleAsync(ProductCreatedEvent @event, CancellationToken cancellationToken = default)
        {
            await Console.Out.WriteLineAsync($"Product created (warehouse) {@event.ProductId}-{@event.ProductName}");
        }
    }   
}
