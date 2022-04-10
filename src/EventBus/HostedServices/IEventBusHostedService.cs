using System.Threading;
using System.Threading.Tasks;

namespace EventBus.HostedServices
{
    public interface IEventBusHostedService
    {
        Task StartAsync(CancellationToken stoppingToken);
        Task StopAsync(CancellationToken stoppingToken);
    }
}
