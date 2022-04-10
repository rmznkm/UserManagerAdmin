using EventBus.Configuration.HostedService;
using EventBus.HostedServices;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace EventBus.Tests.Configuration
{
    public class HostedServiceTests
    {
        [Fact]
        public void AddEventBusHostedServices_Register_IEventBusHostedService()
        {
            // Act && Assert
            var services = new ServiceCollection();
            services.AddEventBusHostedServices();

            services.IsRegisted<IEventBusHostedService>().Should().BeTrue();
        }

        [Fact]
        public void AddEventBusHostedServices_Register_RabbitMqConnectorHostedService()
        {
            // Act && Assert
            var services = new ServiceCollection();
            services.AddEventBusHostedServices();

            services.IsImplementationRegisted<RabbitMqConnectorHostedService>().Should().BeTrue();
        }


        [Fact]
        public void AddEventBusHostedServices_Register_TransportIncomingHostedService()
        {
            // Act && Assert
            var services = new ServiceCollection();
            services.AddEventBusHostedServices();

            services.IsImplementationRegisted<TransportIncomingHostedService>().Should().BeTrue();
        }
    }
}
