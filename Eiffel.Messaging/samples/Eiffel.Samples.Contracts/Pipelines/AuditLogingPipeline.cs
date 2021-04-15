using Eiffel.Messaging.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Eiffel.Samples.Contracts.Pipelines
{
    public class AuditLogingPipeline : IPipelinePostProcessor
    {
        public async Task ProcessAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
        {
            await Console.Out.WriteLineAsync("Audit log!");
        }
    }
}
