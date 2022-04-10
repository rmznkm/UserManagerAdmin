using EventBus.HostedServices;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagerAdmin.TestHelper;
using Xunit;

namespace EventBus.Tests.HostedServices
{
    public class StarterHostedSevicesTests : TestsFor<StarterHostedSevices>
    {
        private List<IEventBusHostedService> eventBusHostedServices = new List<IEventBusHostedService>();
        public override void BeforeInstanceCreated()
        {
            Inject<IEnumerable<IEventBusHostedService>>(eventBusHostedServices);
        }

        [Fact]
        public async Task StartAsync_WhenCalled_InvokesHostedServicesStartAsync()
        {
            // Arrange
            var hostedService = new Mock<IEventBusHostedService>();
            eventBusHostedServices.Add(hostedService.Object);
            eventBusHostedServices.Add(Mock.Of<IEventBusHostedService>());

            // Act
            await Instance.StartAsync(CancellationToken.None);

            // Assert
            hostedService.Verify(c => c.StartAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task StartAsync_IfServiceStartThrowsException_ThrowsFirstException()
        {
            // Arrange
            var hostedService = new Mock<IEventBusHostedService>();
            hostedService.Setup(c => c.StartAsync(It.IsAny<CancellationToken>()))
                .Throws<InvalidOperationException>();

            eventBusHostedServices.Add(hostedService.Object);
            eventBusHostedServices.Add(Mock.Of<IEventBusHostedService>());

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => Instance.StartAsync(CancellationToken.None));
        }

        [Fact]
        public async Task StopAsync_WhenCalled_InvokesHostedServicesStopAsync()
        {
            // Arrange
            var hostedService = new Mock<IEventBusHostedService>();
            eventBusHostedServices.Add(hostedService.Object);
            eventBusHostedServices.Add(Mock.Of<IEventBusHostedService>());

            // Act
            await Instance.StopAsync(CancellationToken.None);

            // Assert
            hostedService.Verify(c => c.StopAsync(It.IsAny<CancellationToken>()), Times.Once());
        }


        [Fact]
        public async Task StartAsync_IfServiceStopThrowsException_ThrowsAggregateExceptionException()
        {
            // Arrange
            var hostedService = new Mock<IEventBusHostedService>();
            hostedService.Setup(c => c.StopAsync(It.IsAny<CancellationToken>()))
                .Throws<InvalidOperationException>();

            eventBusHostedServices.Add(hostedService.Object);
            eventBusHostedServices.Add(Mock.Of<IEventBusHostedService>());

            // Act & Assert
            await Assert.ThrowsAsync<AggregateException>(() => Instance.StopAsync(CancellationToken.None));
        }
    }
}
