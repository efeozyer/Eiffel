using Eiffel.Messaging.Abstractions;
using Eiffel.Samples.Contracts.Events;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Eiffel.Samples.Messaging.Kafka
{
    public class WorkerService : BackgroundService
    {
        private readonly ILogger<WorkerService> _logger;
        private readonly IEventBus _eventBus;

        public WorkerService(ILogger<WorkerService> logger, IEventBus eventBus)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker service started");
            await _eventBus.PublishAsync(new ProductCreatedEvent(1, "Test"), stoppingToken);
            await _eventBus.SubscribeAsync<ProductCreatedEvent>(stoppingToken);
        }
    }
}
