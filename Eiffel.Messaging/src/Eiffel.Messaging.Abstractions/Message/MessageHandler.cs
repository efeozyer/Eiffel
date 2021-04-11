using System.Threading;
using System.Threading.Tasks;

namespace Eiffel.Messaging.Abstractions
{
    public abstract class MessageHandler<TMesssage>
        where TMesssage : Message
    {
        public abstract Task HandleAsync(TMesssage message, CancellationToken cancellationToken = default);
    }
}
