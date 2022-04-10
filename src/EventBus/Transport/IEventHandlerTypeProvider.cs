namespace EventBus.Transport
{
    public interface IEventHandlerTypeProvider
    {
        IDictionary<Type, IList<Type>> GetEventBasedHandlers();

        KeyValuePair<Type, IList<Type>> GetEventBasedHandlers(string eventName);
    }
}
