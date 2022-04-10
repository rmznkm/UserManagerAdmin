namespace EventBus.Event
{
    public interface IEvent
    {
        Guid Id { get; }
    }

    public interface IEvent<TModel> : IEvent
    {
       public TModel Model { get; set; }
    }
}
