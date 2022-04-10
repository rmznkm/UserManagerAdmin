using EventBus.HostedServices;
using Microsoft.Extensions.DependencyInjection;

namespace EventBus.Configuration.HostedService
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEventBusHostedServices(this IServiceCollection services)
        {
            services.AddSingleton<IEventBusHostedService, RabbitMqConnectorHostedService>();
            services.AddSingleton<IEventBusHostedService, TransportIncomingHostedService>();
            return services;
        }
    }
}
