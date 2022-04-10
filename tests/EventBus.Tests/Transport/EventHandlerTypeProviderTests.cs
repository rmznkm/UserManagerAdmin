using EventBus.Event;
using EventBus.Transport;
using FluentAssertions;
using UserManagerAdmin.TestHelper;
using Xunit;

namespace EventBus.Tests.Transport
{
    public class FooEvent : IEvent<object>
    {
        public object Model { get; set; }

        public Guid Id { get; set; }
    }

    public class FooEventHandler : IEventHandler<FooEvent>
    {
        public Task HandleAsync(FooEvent @event)
        {
            return Task.CompletedTask;
        }
    }

    public class EventHandlerTypeProviderTests : TestsFor<EventHandlerTypeProvider>
    {
        public override void BeforeInstanceCreated()
        {
            Inject<ITypeNameProvider>(new TypeNameProvider());
            base.BeforeInstanceCreated();
        }

        [Fact]
        public void GetEventBasedHandlers_Should_ReturnFoo()
        {
            // Act 
            var result = Instance.GetEventBasedHandlers();

            //Assert
            result.Any(x => x.Key == typeof(FooEvent)).Should().BeTrue();
        }

        [Fact]
        public void GetEventBasedHandlersByString_Should_ReturnFoo()
        {
            // Act 
            var result = Instance.GetEventBasedHandlers(typeof(FooEvent).FullName);

            //Assert
            result.Key.Should().Be(typeof(FooEvent));
        }
    }
}
