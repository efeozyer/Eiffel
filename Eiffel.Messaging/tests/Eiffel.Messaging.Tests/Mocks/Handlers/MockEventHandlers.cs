using Eiffel.Messaging.Abstractions;
using Eiffel.Messaging.Tests.Mocks.Events;
using System.Threading;
using System.Threading.Tasks;

namespace Eiffel.Messaging.Tests.Mocks.Handlers
{
    public class MockEventHandler1 : EventHandler<MockEvent>
    {
        public override Task HandleAsync(MockEvent @event, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }

    public class MockEventHandler2 : EventHandler<MockEvent>
    {
        public override Task HandleAsync(MockEvent @event, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
