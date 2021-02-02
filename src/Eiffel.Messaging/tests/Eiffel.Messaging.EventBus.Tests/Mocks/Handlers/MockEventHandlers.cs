using Eiffel.Messaging.Abstractions.Event;
using Eiffel.Messaging.EventBus.Tests.Mocks.Events;
using System.Threading;
using System.Threading.Tasks;

namespace Eiffel.Messaging.EventBus.Tests.Mocks.Handlers
{
    public class MockEventHandler1 : IEventHandler<MockEvent>
    {
        public virtual Task HandleAsync(MockEvent @event, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }

    public class MockEventHandler2 : IEventHandler<MockEvent>
    {
        public virtual Task HandleAsync(MockEvent @event, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
