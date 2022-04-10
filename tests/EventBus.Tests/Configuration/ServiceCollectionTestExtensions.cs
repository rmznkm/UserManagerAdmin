using Microsoft.Extensions.DependencyInjection;

namespace EventBus.Tests.Configuration
{
    public static class ServiceCollectionTestExtensions
    {
        public static bool IsRegisted<T>(this IServiceCollection services)
        {
            return services.Any(c => c.ServiceType == typeof(T));
        }
        public static bool IsImplementationRegisted<T>(this IServiceCollection services)
        {
            return services.Any(c => c.ImplementationType == typeof(T));
        }
    }
}