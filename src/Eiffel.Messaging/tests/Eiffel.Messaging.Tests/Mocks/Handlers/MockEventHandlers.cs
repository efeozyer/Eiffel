using Eiffel.Messaging.Abstractions.Event;
using Eiffel.Messaging.Tests.Mocks.Messages;
using System.Threading;
using System.Threading.Tasks;

namespace Eiffel.Messaging.Tests.Mocks.Handlers
{
    public class MockEventHandler1 : IEventHandler<MockEvent>
    {
        public virtual Task HandleAsync(MockEvent message, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }

    public class MockEventHandler2 : IEventHandler<MockEvent>
    {
        public virtual Task HandleAsync(MockEvent message, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
