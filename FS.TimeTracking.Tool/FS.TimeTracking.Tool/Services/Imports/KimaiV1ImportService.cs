using AutoMapper;
using FS.TimeTracking.Shared.Interfaces.Application.Services;
using FS.TimeTracking.Shared.Interfaces.Repository.Services;
using FS.TimeTracking.Shared.Models.TimeTracking;
using FS.TimeTracking.Tool.Interfaces.Import;
using FS.TimeTracking.Tool.Models.Configurations;
using FS.TimeTracking.Tool.Models.Imports;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimeZoneConverter;

namespace FS.TimeTracking.Tool.Services.Imports
{
    public class KimaiV1ImportService : IKimaiV1ImportService
    {
        private readonly IRepository _repository;
        private readonly ITestDataService _testDataService;
        private readonly IKimaiV1Repository _kimaiV1Repository;
        private readonly IMapper _mapper;
        private readonly KimaiV1ImportConfiguration _importConfiguration;

        public KimaiV1ImportService(IRepository repository, IKimaiV1Repository kimaiV1Repository, ITestDataService testDataService, IMapper mapper, IOptions<KimaiV1ImportConfiguration> importConfiguration)
        {
            _repository = repository;
            _kimaiV1Repository = kimaiV1Repository;
            _testDataService = testDataService;
            _mapper = mapper;
            _importConfiguration = importConfiguration.Value;
        }

        public async Task Import()
        {
            var kimaiCustomers = await _kimaiV1Repository.Get((KimaiV1Customer customer) => customer);
            var customers = _mapper.Map<List<Customer>>(kimaiCustomers).ToList();

            var kimaiProjects = await _kimaiV1Repository.Get((KimaiV1Project project) => project);
            var projects = kimaiProjects
                .Select(kimaiProject =>
                {
                    var project = _mapper.Map<Project>(kimaiProject);
                    project.CustomerId = kimaiCustomers
                        .SingleOrDefault(kimaiCustomer => kimaiCustomer.CustomerId == kimaiProject.CustomerId)?.Id
                        ?? throw new InvalidOperationException($"Kimai customer with ID {kimaiProject.CustomerId} not found, Kimai project ID {kimaiProject.ProjectId}");
                    return project;
                })
                .ToList();

            var kimaiActivities = await _kimaiV1Repository.Get((KimaiV1Activity activity) => activity, includes: new[] { nameof(KimaiV1Activity.Projects) });
            var activities = kimaiActivities
                .SelectMany(kimaiActivity =>
                {
                    if (!kimaiActivity.Projects.Any())
                        return new[] { _mapper.Map<Activity>(kimaiActivity) };

                    return kimaiActivity
                        .Projects
                        .Select((kimaiProject, index) =>
                        {
                            var activity = _mapper.Map<Activity>(kimaiActivity);
                            if (index > 0)
                                activity.Id = Guid.NewGuid();
                            activity.ProjectId = kimaiProjects
                                .SingleOrDefault(x => x.ProjectId == kimaiProject.ProjectId)?.Id
                                ?? throw new InvalidOperationException($"Kimai project with ID {kimaiProject.ProjectId} not found, Kimai activity ID {kimaiActivity.ActivityId}");
                            return activity;
                        });
                })
                .ToList();

            var kimaiTimeSheets = await _kimaiV1Repository.Get((KimaiV1TimeSheet timeSheet) => timeSheet);
            var timeSheets = kimaiTimeSheets
                .Select(kimaiTimeSheet =>
                {
                    var kimaiProject = kimaiProjects.SingleOrDefault(p => p.ProjectId == kimaiTimeSheet.ProjectId);
                    if (kimaiProject == null)
                        throw new InvalidOperationException($"Kimai project with ID {kimaiTimeSheet.ProjectId} not found, Kimai time sheet ID {kimaiTimeSheet.TimeEntryId}");

                    var kimaiActivity = kimaiActivities.SingleOrDefault(x => x.ActivityId == kimaiTimeSheet.ActivityId);
                    if (kimaiActivity == null)
                        throw new InvalidOperationException($"Kimai activity with ID {kimaiTimeSheet.ActivityId} not found, Kimai time sheet ID {kimaiTimeSheet.TimeEntryId}");

                    var kimaiCustomer = kimaiCustomers.SingleOrDefault(c => c.CustomerId == kimaiProject.CustomerId);
                    if (kimaiCustomer == null)
                        throw new InvalidOperationException($"Kimai customer with ID {kimaiProject.CustomerId} not found, Kimai time sheet ID {kimaiTimeSheet.TimeEntryId}");

                    var timeSheet = _mapper.Map<TimeSheet>(kimaiTimeSheet);

                    var kimaiTimeZone = TZConvert.GetTimeZoneInfo(kimaiCustomer.TimeZone);
                    timeSheet.StartDate = TimeZoneInfo.ConvertTime(timeSheet.StartDate, kimaiTimeZone);
                    if (timeSheet.EndDate.HasValue)
                        timeSheet.EndDate = TimeZoneInfo.ConvertTime(timeSheet.EndDate.Value, kimaiTimeZone);

                    timeSheet.ProjectId = kimaiProject.Id;
                    timeSheet.ActivityId = kimaiActivity.Id;

                    return timeSheet;
                })
                .ToList();

            using var transaction = _repository.CreateTransactionScope();
            if (_importConfiguration.TruncateBeforeImport)
                await _testDataService.TruncateData();
            await _repository.BulkAddRange(customers);
            await _repository.BulkAddRange(projects);
            await _repository.BulkAddRange(activities);
            await _repository.BulkAddRange(timeSheets);
            await _repository.SaveChanges();
            transaction.Complete();
        }
    }
}
