using Microsoft.Extensions.Hosting;

namespace EventBus.HostedServices
{
    public class StarterHostedSevices : IHostedService
    {
        private readonly IEnumerable<IEventBusHostedService> eventBusHostedServices;

        public StarterHostedSevices(IEnumerable<IEventBusHostedService> eventBusHostedServices) {
            this.eventBusHostedServices = eventBusHostedServices;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return ExecuteAsync(service => service.StartAsync(cancellationToken));
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return ExecuteAsync(service => service.StopAsync(cancellationToken), throwOnFirstFailure: false);
        }

        private async Task ExecuteAsync(Func<IEventBusHostedService, Task> callback, bool throwOnFirstFailure = true)
        {
            List<Exception> exceptions = null;

            foreach (var service in eventBusHostedServices)
            {
                try
                {
                    await callback(service);
                }
                catch (Exception ex)
                {
                    if (throwOnFirstFailure)
                    {
                        throw;
                    }

                    if (exceptions == null)
                    {
                        exceptions = new List<Exception>();
                    }

                    exceptions.Add(ex);
                }
            }

            // Throw an aggregate exception if there were any exceptions
            if (exceptions != null)
            {
                throw new AggregateException(exceptions);
            }
        }
    }
}
