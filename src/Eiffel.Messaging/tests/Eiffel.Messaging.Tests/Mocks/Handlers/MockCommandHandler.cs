using Eiffel.Messaging.Abstractions.Command;
using Eiffel.Messaging.Tests.Mocks.Messages;
using System.Threading;
using System.Threading.Tasks;

namespace Eiffel.Messaging.Tests.Mocks.Handlers
{
    public class MockCommandHandler : ICommandHandler<MockCommand>, ICommandHandler<MockInvalidCommand>
    {
        public virtual Task HandleAsync(MockCommand message, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public virtual Task HandleAsync(MockInvalidCommand command, CancellationToken cancellationToken)
        {
            if (command is IValidatable cmd)
            {
                cmd.Validate();
            }
            return Task.CompletedTask;
        }
    }
}
