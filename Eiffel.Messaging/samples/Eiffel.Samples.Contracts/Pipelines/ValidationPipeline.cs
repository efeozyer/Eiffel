using Eiffel.Messaging.Abstractions;
using System.Threading;
using System.Threading.Tasks;

namespace Eiffel.Samples.Contracts.Pipelines
{
    public class ValidationPipeline : IPipelinePreProcessor
    {
        public Task ProcessAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
        {
            if (message is IValidatable msg)
            {
                msg.Validate();
            }
            return Task.CompletedTask;
        }
    }
}
