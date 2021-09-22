using System;
using System.Threading;
using System.Threading.Tasks;

namespace PlayCore.Core.HostedService
{
    public interface IBackgroundTaskQueue
    {
        void Queue(Func<CancellationToken, Task> workItem);
        Task<Func<CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken);
    }
}
