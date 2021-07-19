using System.Threading;
using System.Threading.Tasks;

namespace Eiffel.Messaging.Abstractions
{
    public interface IPipelinePreProcessor
    {
        Task ProcessAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default);
    }
}

