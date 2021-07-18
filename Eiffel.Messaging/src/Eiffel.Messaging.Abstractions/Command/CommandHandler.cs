using System.Threading;
using System.Threading.Tasks;

namespace Eiffel.Messaging.Abstractions
{
    public abstract class CommandHandler<TPayload>
        where TPayload : Command
    {
        public abstract Task HandleAsync(TPayload payload, CancellationToken cancellationToken = default);
    }
}

