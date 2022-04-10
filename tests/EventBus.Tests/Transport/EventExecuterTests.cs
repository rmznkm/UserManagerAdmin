using EventBus.Event;
using EventBus.Storage;
using EventBus.Transport;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Text.Json;
using UserManagerAdmin.TestHelper;
using Xunit;

namespace EventBus.Tests.Transport
{
    public class EventExecuterTests : TestsFor<EventExecuter>
    {
        Mock<IDbStorage> mockDbStorage = new Mock<IDbStorage>();
        public class FooModel
        {
            public bool IsThrowException { get; set; }
        }

        public class FooModelEvent : IEvent<FooModel>
        {
            public FooModel Model { get; set; }

            public Guid Id { get; set; }
        }

        public class FooModelEventHandler : IEventHandler<FooModelEvent>
        {

            public Task HandleAsync(FooModelEvent @event)
            {
                if (@event.Model.IsThrowException)
                {
                    throw new Exception("Test Exception");
                }
                return Task.CompletedTask;
            }
        }

        public override void BeforeInstanceCreated()
        {
            var services = new ServiceCollection();
            services.AddSingleton<IDbStorage>(mockDbStorage.Object);
            var serviceProvider = services.BuildServiceProvider();
            Inject<IServiceProvider>(serviceProvider);
            base.BeforeInstanceCreated();
        }

        [Fact]
        public async Task ProcessEventAsync_IfSuccess_CallSuccess()
        {
            //Arrange
            string eventName = "EventName";
            GetMockFor<IEventHandlerTypeProvider>()
                .Setup(x => x.GetEventBasedHandlers(eventName))
                .Returns(new KeyValuePair<Type, IList<Type>>(typeof(FooModelEvent), new List<Type> { typeof(FooModelEventHandler) }));

            bool isSuccess = false;
            bool isError = false;

            var @event = new FooModelEvent { Model = new FooModel() };

            // Act 
            var body = JsonSerializer.SerializeToUtf8Bytes(@event, @event.GetType(), new JsonSerializerOptions { WriteIndented = true });
            await Instance.ProcessEventAsync(eventName, body, () => { isSuccess = true; }, () => { isError = true; });

            //Assert
            isSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task ProcessEventAsync_IfError_CallError()
        {
            //Arrange
            string eventName = "EventName";
            GetMockFor<IEventHandlerTypeProvider>()
                .Setup(x => x.GetEventBasedHandlers(eventName))
                .Returns(new KeyValuePair<Type, IList<Type>>(typeof(FooModelEvent), new List<Type> { typeof(FooModelEventHandler) }));

            bool isSuccess = false;
            bool isError = false;

            var @event = new FooModelEvent { Model = new FooModel { IsThrowException = true } };

            // Act 
            var body = JsonSerializer.SerializeToUtf8Bytes(@event, @event.GetType(), new JsonSerializerOptions { WriteIndented = true });
            try
            {
                await Instance.ProcessEventAsync(eventName, body, () => { isSuccess = true; }, () => { isError = true; });
            }
            catch
            {
                //Nothing to do
            }

            //Assert
            isError.Should().BeTrue();
        }
    }
}
