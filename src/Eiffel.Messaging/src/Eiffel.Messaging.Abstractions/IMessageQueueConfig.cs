namespace Eiffel.Messaging.Abstractions
{
    public interface IMessageQueueConfig
    {
        string Host { get; }
        int Port { get; }
        string Username { get; }
        string Password { get; }
    }
}
