using EventBus.Event;

namespace EventBus.Transport
{
    public class EventHandlerTypeProvider : IEventHandlerTypeProvider
    {
        private Dictionary<Type, IList<Type>> eventBasedHandlers = new Dictionary<Type, IList<Type>>();
        private Dictionary<string, IList<Type>> stringEventBasedHandlers = new Dictionary<string, IList<Type>>();
        private Dictionary<string, Type> stringEvents = new Dictionary<string, Type>();

        public EventHandlerTypeProvider(ITypeNameProvider typeNameProvider)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    var eventHandlers = assembly.GetTypes().Where(t => typeof(IEventHandler).IsAssignableFrom(t) && !t.IsAbstract);
                    foreach (var eventHandler in eventHandlers)
                    {
                        var interfaces = eventHandler.GetInterfaces();
                        foreach (var @interface in interfaces)
                        {
                            if (!typeof(IEventHandler).IsAssignableFrom(@interface))
                            {
                                continue;
                            }
                            var genericArguments = @interface.GetGenericArguments();
                            if (!genericArguments.Any())
                            {
                                continue;
                            }
                            var eventType = genericArguments[0];
                            if (!eventBasedHandlers.ContainsKey(eventType))
                            {
                                eventBasedHandlers.Add(eventType, new List<Type>());
                            }
                            eventBasedHandlers[eventType].Add(eventHandler);

                            var eventName = typeNameProvider.GetTypeName(eventType);
                            if (!stringEventBasedHandlers.ContainsKey(eventName))
                            {
                                stringEventBasedHandlers.Add(eventName, new List<Type>());
                            }
                            stringEventBasedHandlers[eventName].Add(eventHandler);
                            stringEvents.Add(eventName, eventType);
                        }
                    }
                }
                catch
                {
                    //TODO:log 
                }
            }
        }

        public IDictionary<Type, IList<Type>> GetEventBasedHandlers()
        {
            return eventBasedHandlers;
        }

        public KeyValuePair<Type, IList<Type>> GetEventBasedHandlers(string eventName)
        {
            return new KeyValuePair<Type, IList<Type>>(stringEvents[eventName], stringEventBasedHandlers[eventName]);
        }
    }
}
