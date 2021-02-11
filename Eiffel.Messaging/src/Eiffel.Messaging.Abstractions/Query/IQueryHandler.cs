using System.Threading;
using System.Threading.Tasks;

namespace Eiffel.Messaging.Abstractions
{
    public interface IQueryHandler<TQuery, TReply> : IMessageHandler<TQuery, TReply>
        where TQuery : IQuery<TReply> 
    {
        new Task<TReply> HandleAsync(TQuery query, CancellationToken cancellationToken);
    }
}
