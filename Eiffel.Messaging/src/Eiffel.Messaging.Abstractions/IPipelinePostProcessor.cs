using System.Threading;
using System.Threading.Tasks;

namespace Eiffel.Messaging.Abstractions
{
    public interface IPipelinePostProcessor
    {
        Task ProcessAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default);
    }
}
