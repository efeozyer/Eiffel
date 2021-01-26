using System.Threading;
using System.Threading.Tasks;

namespace Eiffel.Messaging.Abstractions.Query
{
    public interface IQueryHandler<TQuery, TReply>
        where TQuery : IMessage<TReply> {
        Task<TReply> HandleAsync(TQuery query, CancellationToken cancellationToken);
    }
}
