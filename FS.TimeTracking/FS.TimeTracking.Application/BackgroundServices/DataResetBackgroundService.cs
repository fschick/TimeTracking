using FS.TimeTracking.Core.Interfaces.Application.Services.Administration;
using FS.TimeTracking.Core.Models.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Application.BackgroundServices;

internal class DataResetBackgroundService : CronBackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    // ReSharper disable once SuggestBaseTypeForParameterInConstructor
    //  Justification: Logger should contain class type as category.
    public DataResetBackgroundService(ILogger<DataResetBackgroundService> logger, IServiceScopeFactory serviceScopeFactory, IOptions<TimeTrackingConfiguration> configuration)
        : base(logger, "data-reset", GetCronJobSchedule(configuration.Value))
        => _serviceScopeFactory = serviceScopeFactory;

    protected override async Task NextExecuteAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var maintenanceService = scope.ServiceProvider.GetRequiredService<IMaintenanceService>();
        await maintenanceService.ResetDatabase();
    }

    private static string GetCronJobSchedule(TimeTrackingConfiguration configuration)
        => configuration.DataReset.Enabled
            ? configuration.DataReset.CronJobSchedule
            : null;
}