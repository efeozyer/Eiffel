using System.Threading;
using System.Threading.Tasks;

namespace Eiffel.Messaging.Abstractions
{
    public abstract class CommandHandler<TCommand>
        where TCommand : Command
    {
        public IDbContext DbContext { get; internal set; }

        public abstract Task<string> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
    }
}
