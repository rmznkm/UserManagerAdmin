using EventBus.Options;
using EventBus.Transport.RabbitMq;
using FluentAssertions;
using Moq;
using RabbitMQ.Client;
using System.Net.Sockets;
using UserManagerAdmin.TestHelper;
using Xunit;

namespace EventBus.Tests.Transport.RabbitMq
{
    public class DefaultRabbitMQPersistentConnectionTests : TestsFor<DefaultRabbitMQPersistentConnection>
    {
        [Fact]
        public void TryConnect_Call_ConnectionFactory()
        {
            //Arrange
            var mockConnection = new Mock<IConnection>();
            GetMockFor<IEventBusRabbitMqConnectionFactory>().Setup(x => x.CreateConnection(It.IsAny<string>()))
                .Returns(mockConnection.Object);
            
            // Act 
            var result = Instance.TryConnect();

            //Assert
            GetMockFor<IEventBusRabbitMqConnectionFactory>().Verify(x => x.CreateConnection(It.IsAny<string>()), Times.AtLeastOnce);
        }

        [Fact]
        public void TryConnect_IfExceptionOccur_MoreThanOne()
        {
            //Arrange
            var mockConnection = new Mock<IConnection>();
            GetMockFor<IEventBusRabbitMqConnectionFactory>().Setup(x => x.CreateConnection(It.IsAny<string>()))
                .Throws<SocketException>();

            GetMockFor<IRabbitMqOptions>().Setup(x => x.RetryCount).Returns(2);
           
            // Act 
            try { Instance.TryConnect(); } catch { }

            //Assert
            GetMockFor<IEventBusRabbitMqConnectionFactory>().Verify(x => x.CreateConnection(It.IsAny<string>()), Times.AtLeast(2));
        }

        [Fact]
        public void IsConnected_IfConnectionNull_ReturnFalse()
        {
            // Arrange & Act 
            var result = Instance.IsConnected;

            //Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void TryConnect_IfIsConnected_ReturnTrue()
        {
            //Arrange
            var mockConnection = new Mock<IConnection>();
            mockConnection.Setup(x => x.IsOpen).Returns(true);
            mockConnection.Setup(x => x.Endpoint).Returns(new AmqpTcpEndpoint { HostName = "TestHosName" });

            GetMockFor<IEventBusRabbitMqConnectionFactory>().Setup(x => x.CreateConnection(It.IsAny<string>()))
                .Returns(mockConnection.Object);
            
            // Act 
            var result = Instance.TryConnect();

            //Assert
            result.Should().BeTrue();
        }
    }
}
