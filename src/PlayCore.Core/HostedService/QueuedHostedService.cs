using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PlayCore.Core.Logger;

namespace PlayCore.Core.HostedService
{
    public class QueuedHostedService : BackgroundService
    {
        private readonly IBasicLogger<QueuedHostedService> _basicLogger;

        public QueuedHostedService(IBackgroundTaskQueue taskQueue, IBasicLogger<QueuedHostedService> basicLogger)
        {
            TaskQueue = taskQueue;
            _basicLogger = basicLogger;
        }

        public IBackgroundTaskQueue TaskQueue { get; }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _basicLogger.LogInformation("Queued Hosted Service is starting.");

            while (!cancellationToken.IsCancellationRequested)
            {
                var workItem = await TaskQueue.DequeueAsync(cancellationToken);

                try
                {
                    await workItem(cancellationToken);
                }
                catch (Exception ex)
                {
                    _basicLogger.LogError(ex, "Error occurred executing {WorkItem}.", nameof(workItem));
                }
            }

            _basicLogger.LogInformation("Queued Hosted Service is stopping.");
        }
    }
}
