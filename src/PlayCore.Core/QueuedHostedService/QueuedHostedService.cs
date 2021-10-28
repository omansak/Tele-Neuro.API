using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PlayCore.Core.Logger;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PlayCore.Core.QueuedHostedService
{
    public class QueuedHostedService : BackgroundService
    {
        private readonly IBasicLogger<QueuedHostedService> _basicLogger;
        public IBackgroundTaskQueue TaskQueue { get; }
        public QueuedHostedService(IBackgroundTaskQueue taskQueue, IBasicLogger<QueuedHostedService> basicLogger)
        {
            TaskQueue = taskQueue;
            _basicLogger = basicLogger;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var workItem = await TaskQueue.DequeueAsync(cancellationToken);

                try
                {
                    await workItem(cancellationToken);
                }
                catch (Exception ex)
                {
                    _basicLogger?.LogException("Error occurred executing {WorkItem}.", ex);
                }
            }
        }
    }
}
