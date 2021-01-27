namespace Eiffel.Messaging.Abstractions.Command
{
    public interface ICommandHandler<in TCommand> : IMessageHandler<TCommand> 
        where TCommand : ICommand 
    {
    }
}
