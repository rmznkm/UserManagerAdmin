using EventBus.Event;
using EventBus.Transport;
using EventBus.Transport.RabbitMq;
using Moq;
using RabbitMQ.Client;
using UserManagerAdmin.TestHelper;
using Xunit;

namespace EventBus.Tests.Transport.RabbitMq
{
    public class RabbitMQTransportTests : TestsFor<RabbitMQTransport>
    {
        public class FooModel { }

        public class FooModelEvent : IEvent<FooModel>
        {
            public FooModel Model { get; set; }

            public Guid Id { get; set; }
        }

        [Fact]
        public void Publish_IfNotConnected_CallConnect()
        {
            //Arrange
            var mockChannel = new Mock<IModel>();
            var mockBasicProperties = new Mock<IBasicProperties>();
            mockChannel.Setup(x => x.CreateBasicProperties()).Returns(mockBasicProperties.Object);
           
            GetMockFor<IRabbitMQPersistentConnection>().Setup(x => x.IsConnected).Returns(false);
            GetMockFor<IRabbitMQPersistentConnection>().Setup(x => x.CreateModel()).Returns(mockChannel.Object);

            // Act 
            Instance.Publish(new FooModelEvent());

            //Assert
            GetMockFor<IRabbitMQPersistentConnection>().Verify(x => x.TryConnect());
        }

        [Fact]
        public void Publish_Call_CallChannelPublish()
        {
            //Arrange
            var mockChannel = new Mock<IModel>();
            var mockBasicProperties = new Mock<IBasicProperties>();
            mockChannel.Setup(x => x.CreateBasicProperties()).Returns(mockBasicProperties.Object);

            GetMockFor<IRabbitMQPersistentConnection>().Setup(x => x.IsConnected).Returns(false);
            GetMockFor<IRabbitMQPersistentConnection>().Setup(x => x.CreateModel()).Returns(mockChannel.Object);

            // Act 
            Instance.Publish(new FooModelEvent());

            //Assert
            mockChannel.Verify(x => x.BasicPublish(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<IBasicProperties>(),It.IsAny<ReadOnlyMemory<byte>>()));
        }



        [Fact]
        public async Task InitializeAsync_IfFirst_CallCreateModel()
        {
            //Arrange
            var mockChannel = new Mock<IModel>();
            GetMockFor<IRabbitMQPersistentConnection>().Setup(x => x.CreateModel()).Returns(mockChannel.Object);

            GetMockFor<IEventHandlerTypeProvider>().Setup(x => x.GetEventBasedHandlers()).Returns(new Dictionary<Type,IList<Type>>());

            // Act 
            await Instance.InitializeAsync();

            //Assert
            GetMockFor<IRabbitMQPersistentConnection>().Verify(x => x.CreateModel(), Times.AtLeastOnce);
        }

        [Fact]
        public async Task InitializeAsync_EachEvent_CallExchangeDeclare()
        {
            //Arrange
            var mockChannel = new Mock<IModel>();
            GetMockFor<IRabbitMQPersistentConnection>().Setup(x => x.CreateModel()).Returns(mockChannel.Object);

            var handlers = new Dictionary<Type, IList<Type>>
            {
                { typeof(FooEvent), new List<Type>() }
            };
            GetMockFor<IEventHandlerTypeProvider>().Setup(x => x.GetEventBasedHandlers()).Returns(handlers);

            // Act 
            await Instance.InitializeAsync();

            //Assert
            mockChannel.Verify(x => x.ExchangeDeclare(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<IDictionary<string, object>>()),Times.AtLeastOnce);
            mockChannel.Verify(x => x.ExchangeBind(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IDictionary<string, object>>()), Times.AtLeastOnce);
        }
    }
}
