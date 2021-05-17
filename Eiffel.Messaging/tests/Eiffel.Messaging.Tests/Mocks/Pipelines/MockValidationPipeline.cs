using Eiffel.Messaging.Abstractions;
using Eiffel.Messaging.Tests.Mocks.Messages;
using System.Threading;
using System.Threading.Tasks;

namespace Eiffel.Messaging.Tests.Mocks.Pipelines
{
    public class MockValidationPipeline : IPipelinePreProcessor
    {
        public virtual Task ProcessAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
        {
            if (message is IValidatable msg)
            {
                msg.Validate();
            }
            return Task.CompletedTask;
        }
    }
}
