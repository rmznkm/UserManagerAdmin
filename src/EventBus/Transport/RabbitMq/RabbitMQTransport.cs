using EventBus.Event;
using EventBus.Options;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System.Net.Sockets;
using System.Text.Json;

namespace EventBus.Transport.RabbitMq
{
    public class RabbitMQTransport : ITransport
    {
        private readonly IEventExecuter eventExecuter;
        private readonly IRabbitMQPersistentConnection persistentConnection;
        private readonly IEventBusOptions eventBusOptions;
        private readonly IEventHandlerTypeProvider eventHandlerTypeProvider;
        private readonly ILogger<RabbitMQTransport> logger;
        private readonly IRabbitMqOptions rabbitMqOptions;
        private readonly ITypeNameProvider typeNameProvider;
        private IModel consumerChannel;

        public RabbitMQTransport(IEventExecuter eventExecuter,
            IEventHandlerTypeProvider eventHandlerTypeProvider,
            ITypeNameProvider typeNameProvider,
            IRabbitMQPersistentConnection persistentConnection,
            IEventBusOptions eventBusOptions,
            IRabbitMqOptions rabbitMqOptions,
            ILogger<RabbitMQTransport> logger)
        {
            this.logger = logger;
            this.eventBusOptions = eventBusOptions;
            this.typeNameProvider = typeNameProvider;
            this.persistentConnection = persistentConnection;
            this.eventHandlerTypeProvider = eventHandlerTypeProvider;
            this.rabbitMqOptions = rabbitMqOptions;
            this.eventExecuter = eventExecuter;
        }

        public void Publish<TModel>(IEvent<TModel> @event)
        {
            if (!persistentConnection.IsConnected)
            {
                persistentConnection.TryConnect();
            }

            var policy = RetryPolicy.Handle<BrokerUnreachableException>()
                .Or<SocketException>()
                .WaitAndRetry(rabbitMqOptions.RetryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                {
                    logger.LogWarning(ex, "Could not publish event: {EventId} after {Timeout}s ({ExceptionMessage})", @event.Id, $"{time.TotalSeconds:n1}", ex.Message);
                });

            var typeName = typeNameProvider.GetTypeName(@event.GetType());
            logger.LogTrace("Creating RabbitMQ channel to publish event: {EventId} ({EventName})", @event.Id, typeName);

            using (var channel = persistentConnection.CreateModel())
            {
                logger.LogTrace("Declaring RabbitMQ exchange to publish event: {EventId}", @event.Id);
                var body = JsonSerializer.SerializeToUtf8Bytes(@event, @event.GetType(), new JsonSerializerOptions { WriteIndented = true });

                policy.Execute(() =>
                {
                    var properties = channel.CreateBasicProperties();
                    properties.DeliveryMode = 2; // persistent

                    logger.LogTrace("Publishing event to RabbitMQ: {EventId}", @event.Id);

                    channel.BasicPublish(exchange: typeName, routingKey: typeName, mandatory: true, basicProperties: properties, body: body);
                });
            }
        }

        public Task InitializeAsync()
        {
            var arguments = new Dictionary<string, object> { { "x-message-ttl", 10000 } };
            var channel = GetConsumerChannel();
            channel.ExchangeDeclare(eventBusOptions.ServiceName, ExchangeType.Fanout, false);
            channel.QueueDeclare(eventBusOptions.ServiceName, true, false, false, arguments);
            channel.QueueBind(eventBusOptions.ServiceName, eventBusOptions.ServiceName, string.Empty);
            var eventBasedHandlers = eventHandlerTypeProvider.GetEventBasedHandlers();
            foreach (var eventBasedHandler in eventBasedHandlers)
            {
                var exchangeName = typeNameProvider.GetTypeName(eventBasedHandler.Key);
                channel.ExchangeDeclare(exchangeName, ExchangeType.Fanout, false);
                channel.ExchangeBind(eventBusOptions.ServiceName, exchangeName, string.Empty);
            }
            StartBasicConsume();
            return Task.CompletedTask;
        }

        private IModel GetConsumerChannel()
        {
            if (consumerChannel != null)
            {
                return consumerChannel;
            }
            consumerChannel = persistentConnection.CreateModel();
            return consumerChannel;
        }

        private void StartBasicConsume()
        {
            logger.LogTrace("Starting RabbitMQ basic consume");

            var channel = GetConsumerChannel();
            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (model, ea) =>
            {
                var defaultBasicConsumer = (IBasicConsumer)model;
                var payloadData = ea.Body.ToArray();
                var messageHeaders = ea.BasicProperties.Headers;
                var deliveryTag = ea.DeliveryTag;
                var eventName = ea.Exchange;

                var onError = () => defaultBasicConsumer.Model.BasicNack(deliveryTag, multiple: false, requeue: true);
                var onSuccess = () => defaultBasicConsumer.Model.BasicAck(deliveryTag, multiple: false);

                _ = eventExecuter.ProcessEventAsync(eventName, payloadData, onSuccess, onError);
            };
            //TODO: Manage RabbitMQ Cluster Down and And Connection
            //consumer.ConsumerCancelled += Consumer_ConsumerCancelled;
            //consumer.Registered += Consumer_Registered;
            //consumer.Shutdown += Consumer_Shutdown;
            //consumer.Unregistered += Consumer_Unregistered;
            channel.BasicQos(0, 100, false);
            channel.BasicConsume(queue: eventBusOptions.ServiceName, autoAck: false, consumer: consumer);
        }
    }
}
