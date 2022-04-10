namespace EventBus.Options
{
    public class RabbitMqOptions : IRabbitMqOptions
    {
        public string HostName { get; set; } = "localhost";

        public string UserName { get; set; } = "guest";

        public string Password { get; set; } = "guest";

        public int Port { get; set; } = 5672;

        public bool TlsEnabled { get; set; } = false;

        public int RetryCount { get; set; } = 5;
    }
}
