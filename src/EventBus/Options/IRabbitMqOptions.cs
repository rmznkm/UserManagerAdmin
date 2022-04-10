namespace EventBus.Options
{
    public interface IRabbitMqOptions
    {
        string HostName { get; }
        string UserName { get; }
        string Password { get; }
        int Port { get; }
        bool TlsEnabled { get; }
        int RetryCount { get; }
    }
}
