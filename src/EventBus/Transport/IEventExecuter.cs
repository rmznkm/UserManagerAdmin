namespace EventBus.Transport
{
    public interface IEventExecuter
    {
        Task ProcessEventAsync(string eventName, byte[] payload, Action onSuccess, Action onException);
    }
}
