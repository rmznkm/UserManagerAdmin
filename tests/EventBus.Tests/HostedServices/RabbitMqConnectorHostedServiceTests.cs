using EventBus.HostedServices;
using EventBus.Transport.RabbitMq;
using Moq;
using UserManagerAdmin.TestHelper;
using Xunit;

namespace EventBus.Tests.HostedServices
{
    public class RabbitMqConnectorHostedServiceTests : TestsFor<RabbitMqConnectorHostedService>
    {
        [Fact]
        public async Task StartAsync_EveryCall_CallTryConnect()
        {
            // Act 
            await Instance.StartAsync(CancellationToken.None);

            //Assert
            GetMockFor<IRabbitMQPersistentConnection>().Verify(x => x.TryConnect(), Times.Once);
        }

        [Fact]
        public async Task StopAsync_EveryCall_RunWithoutExeption()
        {
            // Act && Assert
            await Instance.StopAsync(CancellationToken.None);
        }
    }
}
