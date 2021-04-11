using Eiffel.Messaging.Abstractions;
using Eiffel.Messaging.Tests.Mocks.Messages;
using System.Threading;
using System.Threading.Tasks;

namespace Eiffel.Messaging.Tests.Mocks.Handlers
{
    public class MockMessageHandler : MessageHandler<MockMessage>
    {
        public override Task HandleAsync(MockMessage message, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }

    public class MockValidatableMessageHandler : MessageHandler<MockValidatableMessage>
    {
        public override Task HandleAsync(MockValidatableMessage message, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}
