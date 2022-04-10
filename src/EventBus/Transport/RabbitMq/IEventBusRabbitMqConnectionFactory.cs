using RabbitMQ.Client;

namespace EventBus.Transport.RabbitMq
{
    public interface IEventBusRabbitMqConnectionFactory
    {
        IConnection CreateConnection(string clientProvidedName);
    }
}
