using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using EventBus.Options;

namespace EventBus.Configuration.Options
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddOptions(this IServiceCollection services, IConfiguration configuration)
        {
            var eventBusOptions = configuration.GetSection("EventBus")?.Get<EventBusOptions>() ?? new EventBusOptions();
            var rabbitMqOptions = configuration.GetSection("RabbitMq")?.Get<RabbitMqOptions>() ?? new RabbitMqOptions();
            var dbOptions = configuration.GetSection("Storage")?.Get<DbOptions>() ?? new DbOptions();

            services.AddSingleton<IEventBusOptions>(s => eventBusOptions);
            services.AddSingleton<IRabbitMqOptions>(s => rabbitMqOptions);
            services.AddSingleton<IDbOptions>(s => dbOptions);
            return services;
        }
    }
}
