using EventBus.Transport;
using EventBus.Transport.RabbitMq;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace EventBus.Tests.Configuration
{
    public class TransportTests
    {
        [Fact]
        public void AddRabbitMq_Register()
        {
            // Act 
            var services = new ServiceCollection();
            services.AddRabbitMq();

            //Assert
            services.IsRegisted<IEventBusRabbitMqConnectionFactory>().Should().BeTrue();
            services.IsRegisted<IRabbitMQPersistentConnection>().Should().BeTrue();
        }

        [Fact]
        public void AddTransport_Register()
        {
            // Act
            var services = new ServiceCollection();
            services.AddTransport();

            //Assert
            services.IsRegisted<ITypeNameProvider>().Should().BeTrue();
            services.IsRegisted<IEventHandlerTypeProvider>().Should().BeTrue();
            services.IsRegisted<ITransport>().Should().BeTrue();
        }
    }
}
