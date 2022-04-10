using EventBus.Configuration.Options;
using EventBus.Options;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace EventBus.Tests.Configuration
{
    public class OptionsTests
    {
        [Fact]
        public void AddOptions_Register_EventBusOptions()
        {
            // Arrange
            var testServiceName = "TestServiceName";
            var dic = new Dictionary<string, string>
            {
                {"EventBus:ServiceName", testServiceName},
            };

            var builder = new ConfigurationBuilder();
            builder.AddInMemoryCollection(dic);
            var configuration = builder.Build();

            var services = new ServiceCollection();

            // Act
            services.AddOptions(configuration);
            var serviceProvider = services.BuildServiceProvider();

            // Assert
            var eventBusOptions = serviceProvider.GetRequiredService<IEventBusOptions>();
            eventBusOptions.ServiceName.Should().Be(testServiceName);
        }


        [Fact]
        public void AddOptions_Register_RabbitMqOptions()
        {
            // Arrange
            string hostName = "TestHOSTNAME";
            var dic = new Dictionary<string, string>
            {
                {"RabbitMq:HostName", hostName},
            };

            var builder = new ConfigurationBuilder();
            builder.AddInMemoryCollection(dic);
            var configuration = builder.Build();

            var services = new ServiceCollection();
            // Act
            services.AddOptions(configuration);
            var serviceProvider = services.BuildServiceProvider();
            var rabbitMqOptions = serviceProvider.GetRequiredService<IRabbitMqOptions>();

            // Assert
            rabbitMqOptions.HostName.Should().Be(hostName);
        }


        [Fact]
        public void AddOptions_Register_Storage()
        {
            // Arrange
            string connectionString = "TestCONNECTIONSTRING";
            var dic = new Dictionary<string, string>
            {
                {"Storage:ConnectionString", connectionString},
            };

            var builder = new ConfigurationBuilder();
            builder.AddInMemoryCollection(dic);
            var configuration = builder.Build();
            var services = new ServiceCollection();

            // Act
            services.AddOptions(configuration);
            var serviceProvider = services.BuildServiceProvider();
            var dbOptions = serviceProvider.GetRequiredService<IDbOptions>();

            //Assert
            dbOptions.ConnectionString.Should().Be(connectionString);
        }
    }
}
