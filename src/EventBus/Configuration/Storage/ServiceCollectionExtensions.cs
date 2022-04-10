using EventBus.Storage;
using EventBus.Storage.Npsql;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace EventBus.Configuration.Storage
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddStorage(this IServiceCollection services)
        {
            services.TryAddTransient<IDbConnectionProvider, NpsqlDbConnectionProvider>();
            services.TryAddScoped<IDbStorage, DbStorage>();
            return services;
        }
    }
}
