using EventBus.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace EventBus.Transport
{
    public class EventExecuter : IEventExecuter
    {
        private readonly IServiceProvider serviceProvider;
        private readonly IEventHandlerTypeProvider eventHandlerTypeProvider;
        private readonly ILogger<EventExecuter> logger;

        public EventExecuter(IServiceProvider serviceProvider, 
            IEventHandlerTypeProvider eventHandlerTypeProvider,
            ILogger<EventExecuter> logger)
        {
            this.serviceProvider = serviceProvider;
            this.eventHandlerTypeProvider = eventHandlerTypeProvider;
            this.logger = logger;
        }

        public async Task ProcessEventAsync(string eventName, byte[] payload, Action onSuccess, Action onException)
        {
            var message = Encoding.UTF8.GetString(payload);
            var typeAndHandlers = eventHandlerTypeProvider.GetEventBasedHandlers(eventName);
            logger.LogTrace("Processing RabbitMQ event: {EventName}", eventName);

            foreach (var handler in typeAndHandlers.Value)
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    var storage = scope.ServiceProvider.GetRequiredService<IDbStorage>();
                    storage.BeginTransaction();
                    try
                    {
                        var handlerObj = ActivatorUtilities.CreateInstance(scope.ServiceProvider, handler);
                        var @event = JsonSerializer.Deserialize(message, typeAndHandlers.Key, new JsonSerializerOptions { WriteIndented = true });
                        var handleMethod = handler.GetMethod("HandleAsync");
                        var task = (Task)handleMethod.Invoke(handlerObj, new[] { @event });
                        await task;
                        storage.CommitTransaction();
                    }
                    catch
                    {                       
                        storage.RollbackTransaction();
                        onException();
                        throw;
                    }
                    finally
                    {
                        if (storage != null)
                        {
                            storage.Close();
                            storage.Dispose();
                            storage = null;
                        }
                    }
                }
            }
            onSuccess();
        }
    }
}
