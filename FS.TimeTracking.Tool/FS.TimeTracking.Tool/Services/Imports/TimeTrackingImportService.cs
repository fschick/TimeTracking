using FS.TimeTracking.Core.Interfaces.Repository.Services.Database;
using FS.TimeTracking.Core.Models.Application.MasterData;
using FS.TimeTracking.Core.Models.Application.TimeTracking;
using FS.TimeTracking.Tool.Interfaces.Import;
using FS.TimeTracking.Tool.Models.Configurations;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FS.TimeTracking.Tool.Services.Imports;

internal class TimeTrackingImportService : ITimeTrackingImportService
{
    private readonly IDbRepository _dbRepository;
    private readonly ITimeTrackingImportRepository _importRepository;
    private readonly TimeTrackingImportConfiguration _importConfiguration;
    private readonly IDbMigrationService _dbMigrationService;

    public TimeTrackingImportService(IDbRepository dbRepository, ITimeTrackingImportRepository importRepository, IOptions<TimeTrackingImportConfiguration> importConfiguration, IDbMigrationService dbMigrationService)
    {
        _dbRepository = dbRepository;
        _importRepository = importRepository;
        _dbMigrationService = dbMigrationService;
        _importConfiguration = importConfiguration.Value;
    }

    public async Task Import()
    {
        var pendingMigrations = string.Join(",", await _importRepository.GetPendingMigrations());
        if (pendingMigrations.Any())
            throw new InvalidOperationException("Source database is not up to date. Please update the application running on source database first.");

        var settings = await _importRepository.Get((Setting x) => x);
        var holidays = await _importRepository.Get((Holiday x) => x);
        var customers = await _importRepository.Get((Customer x) => x);
        var projects = await _importRepository.Get((Project x) => x);
        var activities = await _importRepository.Get((Activity x) => x);
        var orders = await _importRepository.Get((Order x) => x);
        var timeSheets = await _importRepository.Get((TimeSheet x) => x);

        await _dbMigrationService.MigrateDatabase(_importConfiguration.TruncateBeforeImport);
        using var transaction = _dbRepository.CreateTransactionScope();
        await _dbRepository.BulkAddRange(settings);
        await _dbRepository.BulkAddRange(holidays);
        await _dbRepository.BulkAddRange(customers);
        await _dbRepository.BulkAddRange(projects);
        await _dbRepository.BulkAddRange(activities);
        await _dbRepository.BulkAddRange(orders);
        await _dbRepository.BulkAddRange(timeSheets);
        await _dbRepository.SaveChanges();
        transaction.Complete();
    }
}