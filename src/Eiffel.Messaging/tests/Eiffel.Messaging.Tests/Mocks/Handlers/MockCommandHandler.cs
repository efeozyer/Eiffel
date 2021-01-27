using Eiffel.Messaging.Abstractions.Command;
using Eiffel.Messaging.Tests.Mocks.Messages;
using System.Threading;
using System.Threading.Tasks;

namespace Eiffel.Messaging.Tests.Mocks.Handlers
{
    public class MockCommandHandler : ICommandHandler<MockCommand>
    {
        public Task HandleAsync(MockCommand message, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
