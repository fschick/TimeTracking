using FS.TimeTracking.Core.Interfaces.Application.Services.Shared;
using FS.TimeTracking.Core.Interfaces.Repository.Services;
using FS.TimeTracking.Core.Models.Application.MasterData;
using FS.TimeTracking.Core.Models.Application.TimeTracking;
using FS.TimeTracking.Tool.Interfaces.Import;
using FS.TimeTracking.Tool.Models.Configurations;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace FS.TimeTracking.Tool.Services.Imports;

internal class TimeTrackingImportService : ITimeTrackingImportService
{
    private readonly IRepository _repository;
    private readonly ITimeTrackingImportRepository _importRepository;
    private readonly ITestDataService _testDataService;
    private readonly TimeTrackingImportConfiguration _importConfiguration;
    private readonly IDatabaseMigrationService _databaseMigrationService;

    public TimeTrackingImportService(IRepository repository, ITimeTrackingImportRepository importRepository, ITestDataService testDataService, IOptions<TimeTrackingImportConfiguration> importConfiguration, IDatabaseMigrationService databaseMigrationService)
    {
        _repository = repository;
        _importRepository = importRepository;
        _testDataService = testDataService;
        _databaseMigrationService = databaseMigrationService;
        _importConfiguration = importConfiguration.Value;
    }

    public async Task Import()
    {
        var settings = await _importRepository.Get((Setting x) => x);
        var holidays = await _importRepository.Get((Holiday x) => x);
        var customers = await _importRepository.Get((Customer x) => x);
        var projects = await _importRepository.Get((Project x) => x);
        var activities = await _importRepository.Get((Activity x) => x);
        var orders = await _importRepository.Get((Order x) => x);
        var timeSheets = await _importRepository.Get((TimeSheet x) => x);

        _databaseMigrationService.MigrateDatabase(_importConfiguration.TruncateBeforeImport);
        using var transaction = _repository.CreateTransactionScope();
        await _repository.BulkAddRange(settings);
        await _repository.BulkAddRange(holidays);
        await _repository.BulkAddRange(customers);
        await _repository.BulkAddRange(projects);
        await _repository.BulkAddRange(activities);
        await _repository.BulkAddRange(orders);
        await _repository.BulkAddRange(timeSheets);
        await _repository.SaveChanges();
        transaction.Complete();
    }
}