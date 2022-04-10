using EventBus.Options;
using RabbitMQ.Client;
using System.Net.Security;
using System.Security.Authentication;

namespace EventBus.Transport.RabbitMq
{
    public class EventBusRabbitMqConnectionFactory : IEventBusRabbitMqConnectionFactory
    {
        private readonly IRabbitMqOptions rabbitMqOptions;
        private readonly ConnectionFactory connectionFactory = new ConnectionFactory();

        public EventBusRabbitMqConnectionFactory(IRabbitMqOptions rabbitMqOptions)
        {
            this.rabbitMqOptions = rabbitMqOptions;
        }

        private void BuildConnectionFactory()
        {
            connectionFactory.HostName = rabbitMqOptions.HostName;
            connectionFactory.UserName = rabbitMqOptions.UserName;
            connectionFactory.Password = rabbitMqOptions.Password;
            connectionFactory.Port = rabbitMqOptions.Port;
            connectionFactory.Ssl.Enabled = rabbitMqOptions.TlsEnabled;
            connectionFactory.Ssl.Version = SslProtocols.Tls12;
            connectionFactory.Ssl.AcceptablePolicyErrors = SslPolicyErrors.RemoteCertificateChainErrors | SslPolicyErrors.RemoteCertificateNameMismatch | SslPolicyErrors.RemoteCertificateNotAvailable;
        }

        public IConnection CreateConnection(string clientProvidedName)
        {
            BuildConnectionFactory();
            var rabbitMqHostNames = connectionFactory.HostName.Split(",");
            return connectionFactory.CreateConnection(rabbitMqHostNames, clientProvidedName);
        }
    }
}
