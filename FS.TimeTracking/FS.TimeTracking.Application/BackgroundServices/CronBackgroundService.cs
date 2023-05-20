using Cronos;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Application.BackgroundServices;

internal abstract class CronBackgroundService : BackgroundService
{
    private readonly CronExpression _cronScheduler;

    protected CronBackgroundService(ILogger logger, string jobName, string cronExpression)
    {
        var cronJobScheduled = !string.IsNullOrWhiteSpace(cronExpression);
        if (cronJobScheduled)
        {
            _cronScheduler = CronExpression.Parse(cronExpression);
            logger.LogInformation($"Running background job '{jobName}' as cron job schedule '{cronExpression}'");
        }
        else
            logger.LogInformation($"Background job '{jobName}' not running, no cron job schedule specified");
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        if (_cronScheduler == null)
            return;

        var nextRun = _cronScheduler.GetNextOccurrence(DateTimeOffset.Now, TimeZoneInfo.Local);
        while (nextRun.HasValue && !cancellationToken.IsCancellationRequested)
        {
            var now = DateTimeOffset.Now;
            if (now > nextRun)
            {
                var _ = NextExecuteAsync(cancellationToken);
                nextRun = _cronScheduler.GetNextOccurrence(DateTimeOffset.Now, TimeZoneInfo.Local);
            }

            await Task.Delay(1000, cancellationToken);
        }
    }

    protected abstract Task NextExecuteAsync(CancellationToken cancellationToken);
}