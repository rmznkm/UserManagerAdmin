using EventBus.HostedServices;
using EventBus.Transport;
using Moq;
using UserManagerAdmin.TestHelper;
using Xunit;

namespace EventBus.Tests.HostedServices
{
    public class TransportIncomingHostedServiceTests : TestsFor<TransportIncomingHostedService>
    {
        [Fact]
        public async Task StartAsync_EveryCall_CallInitializeAsync()
        {
            // Act 
            await Instance.StartAsync(CancellationToken.None);

            //Assert
            GetMockFor<ITransport>().Verify(x => x.InitializeAsync(), Times.Once);
        }

        [Fact]
        public async Task StopAsync_EveryCall_RunWithoutExeption()
        {
            // Act && Assert
            await Instance.StopAsync(CancellationToken.None);
        }
    }
}
