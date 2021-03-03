namespace Eiffel.Messaging.Abstractions
{
    public interface IMessageQueueClientConfig
    {
        public string Name { get; }
        bool IsEnabled { get; set; }
        bool EnableConsoleLogging { get; set; }
    }
}
