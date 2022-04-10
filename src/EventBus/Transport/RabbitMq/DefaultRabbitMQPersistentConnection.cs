using EventBus.Options;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System.Net.Sockets;

namespace EventBus.Transport.RabbitMq
{
    public class DefaultRabbitMQPersistentConnection : IRabbitMQPersistentConnection
    {
        private readonly IEventBusRabbitMqConnectionFactory connectionFactory;
        private readonly ILogger<DefaultRabbitMQPersistentConnection> logger;
        private readonly string machineName = Environment.MachineName;
        private readonly IEventBusOptions eventBusOptions;
        private readonly IRabbitMqOptions rabbitMqOptions;
        IConnection connection;
        bool disposed;

        object sync_root = new object();

        public DefaultRabbitMQPersistentConnection(IEventBusOptions eventBusOptions, 
            IEventBusRabbitMqConnectionFactory connectionFactory,
            IRabbitMqOptions rabbitMqOptions,
            ILogger<DefaultRabbitMQPersistentConnection> logger)
        {
            this.eventBusOptions = eventBusOptions;
            this.rabbitMqOptions = rabbitMqOptions;
            this.connectionFactory = connectionFactory;
            this.logger = logger;
        }

        public bool IsConnected
        {
            get
            {
                return connection != null && connection.IsOpen && !disposed;
            }
        }

        public IModel CreateModel()
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("No RabbitMQ connections are available to perform this action");
            }

            return connection.CreateModel();
        }

        public bool TryConnect()
        {
            logger.LogInformation("RabbitMQ Client is trying to connect");

            lock (sync_root)
            {
                var policy = RetryPolicy.Handle<SocketException>()
                    .Or<BrokerUnreachableException>()
                    .WaitAndRetry(rabbitMqOptions.RetryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                    {
                        logger.LogWarning(ex, "RabbitMQ Client could not connect after {TimeOut}s ({ExceptionMessage})", $"{time.TotalSeconds:n1}", ex.Message);
                    }
                );

                policy.Execute(() => { connection = connectionFactory.CreateConnection($"{eventBusOptions.ServiceName}_{machineName}"); });

                if (IsConnected)
                {
                    connection.ConnectionShutdown += OnConnectionShutdown;
                    connection.CallbackException += OnCallbackException;
                    connection.ConnectionBlocked += OnConnectionBlocked;

                    logger.LogInformation("RabbitMQ Client acquired a persistent connection to '{HostName}' and is subscribed to failure events", connection.Endpoint.HostName);
                    return true;
                }
                else
                {
                    logger.LogCritical("FATAL ERROR: RabbitMQ connections could not be created and opened");
                    return false;
                }
            }
        }

        void OnConnectionBlocked(object sender, ConnectionBlockedEventArgs e)
        {
            if (disposed)
            {
                return;
            }

            logger.LogWarning("A RabbitMQ connection is shutdown. Trying to re-connect...");
            TryConnect();
        }

        void OnCallbackException(object sender, CallbackExceptionEventArgs e)
        {
            if (disposed) { 
                return;
            }

            logger.LogWarning("A RabbitMQ connection throw exception. Trying to re-connect...");
            TryConnect();
        }

        void OnConnectionShutdown(object sender, ShutdownEventArgs reason)
        {
            if (disposed) { 
                return;
            }

            logger.LogWarning("A RabbitMQ connection is on shutdown. Trying to re-connect...");
            TryConnect();
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }
            disposed = true;

            try
            {
                if (connection != null)
                {
                    connection.ConnectionShutdown -= OnConnectionShutdown;
                    connection.CallbackException -= OnCallbackException;
                    connection.ConnectionBlocked -= OnConnectionBlocked;
                    connection.Dispose();
                }
            }
            catch (IOException ex)
            {
                logger.LogCritical(ex.ToString());
            }
        }
    }
}
