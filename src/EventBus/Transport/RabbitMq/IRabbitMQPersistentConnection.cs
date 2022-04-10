using RabbitMQ.Client;

namespace EventBus.Transport.RabbitMq
{
    public interface IRabbitMQPersistentConnection : IDisposable
    {
        bool IsConnected { get; }

        bool TryConnect();

        IModel CreateModel();
    }

}
