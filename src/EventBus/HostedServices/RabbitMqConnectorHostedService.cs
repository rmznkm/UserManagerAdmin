using EventBus.Transport.RabbitMq;

namespace EventBus.HostedServices
{
    public class RabbitMqConnectorHostedService : IEventBusHostedService
    {
        private readonly IRabbitMQPersistentConnection rabbitMQPersistentConnection;

        public RabbitMqConnectorHostedService(IRabbitMQPersistentConnection rabbitMQPersistentConnection) {
            this.rabbitMQPersistentConnection = rabbitMQPersistentConnection;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            rabbitMQPersistentConnection.TryConnect();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            return Task.CompletedTask;
        }
    }
}
