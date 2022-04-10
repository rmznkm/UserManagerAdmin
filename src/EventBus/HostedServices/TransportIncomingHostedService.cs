using EventBus.Transport;

namespace EventBus.HostedServices
{
    public class TransportIncomingHostedService : IEventBusHostedService
    {
        private readonly ITransport transport;
        public TransportIncomingHostedService(ITransport transport)
        {
            this.transport = transport;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            return transport.InitializeAsync();
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            return Task.CompletedTask;
        }
    }
}
