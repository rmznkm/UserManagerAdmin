using EventBus.Event;

namespace EventBus.Transport
{
    public interface ITransport {
        Task InitializeAsync();
        void Publish<TModel>(IEvent<TModel> @event);
    }
}
