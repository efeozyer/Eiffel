using Eiffel.Messaging.Abstractions;
using Eiffel.Messaging.Tests.Mocks.Messages;
using System.Threading;
using System.Threading.Tasks;

namespace Eiffel.Messaging.Tests.Mocks.Handlers
{
    public class MockCommandHandler : CommandHandler<MockCommand>
    {
        public override Task<string> HandleAsync(MockCommand command, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(string.Empty);
        }
    }

    public class MockInvalidCommandHandler : CommandHandler<MockInvalidCommand>
    {
        public override Task<string> HandleAsync(MockInvalidCommand command, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }
    }
}
