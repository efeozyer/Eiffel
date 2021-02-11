﻿using System.Threading;
using System.Threading.Tasks;

namespace Eiffel.Messaging.Abstractions
{
    public interface ICommandHandler<in TCommand> : IMessageHandler<TCommand> 
        where TCommand : ICommand 
    {
        new Task HandleAsync(TCommand command, CancellationToken cancellationToken);
    }
}