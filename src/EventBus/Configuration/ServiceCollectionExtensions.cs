using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using EventBus.Transport;
using EventBus.Configuration.Options;
using EventBus.Configuration.HostedService;
using EventBus.HostedServices;
using EventBus.Configuration.Storage;

namespace EventBus.Configuration
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEventBus(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions(configuration)
                    .AddRabbitMq()
                    .AddTransport()
                    .AddStorage()
                    .AddEventBusHostedServices();

            services.AddHostedService<StarterHostedSevices>();

            return services;
        }
    }
}
