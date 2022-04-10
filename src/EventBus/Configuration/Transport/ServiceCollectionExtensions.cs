using EventBus.Transport.RabbitMq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace EventBus.Transport
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRabbitMq(this IServiceCollection services)
        {
            services.TryAddSingleton<IEventBusRabbitMqConnectionFactory, EventBusRabbitMqConnectionFactory>();
            services.TryAddSingleton<IRabbitMQPersistentConnection, DefaultRabbitMQPersistentConnection>();
            return services;
        }

        public static IServiceCollection AddTransport(this IServiceCollection services)
        {
            services.AddSingleton<ITypeNameProvider, TypeNameProvider>();
            services.AddSingleton<IEventHandlerTypeProvider, EventHandlerTypeProvider>();
            services.TryAddSingleton<ITransport, RabbitMQTransport>();
            services.TryAddSingleton<IEventExecuter, EventExecuter>();
            return services;
        }
    }
}
