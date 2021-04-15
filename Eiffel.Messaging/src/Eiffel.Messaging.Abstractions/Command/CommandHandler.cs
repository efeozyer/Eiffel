using System.Threading;
using System.Threading.Tasks;

namespace Eiffel.Messaging.Abstractions
{
    public abstract class CommandHandler<TCommand>
        where TCommand : Command
    {
        public abstract Task<string> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
    }
}

