using FS.FilterExpressionCreator.Abstractions.Models;
using FS.FilterExpressionCreator.Filters;
using FS.TimeTracking.Shared.DTOs.MasterData;
using FS.TimeTracking.Shared.DTOs.Shared;
using FS.TimeTracking.Shared.DTOs.TimeTracking;
using FS.TimeTracking.Shared.Enums;
using FS.TimeTracking.Shared.Extensions;
using FS.TimeTracking.Shared.Interfaces.Application.Services.MasterData;
using FS.TimeTracking.Shared.Interfaces.Application.Services.Shared;
using FS.TimeTracking.Shared.Interfaces.Repository.Services;
using FS.TimeTracking.Shared.Models.Application.MasterData;
using FS.TimeTracking.Shared.Models.Application.TimeTracking;
using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Application.Services.Shared;
/// <inheritdoc />
public class WorkdayService : IWorkdayService
{
    private readonly IRepository _repository;
    private readonly ISettingService _settingService;
    private readonly AsyncLazy<List<HolidayDto>> _holidays;

    /// <summary>
    /// Initializes a new instance of the <see cref="WorkdayService" /> class.
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <param name="settingService">The setting service.</param>
    public WorkdayService(IRepository repository, ISettingService settingService)
    {
        _repository = repository;
        _settingService = settingService;
        _holidays = new AsyncLazy<List<HolidayDto>>(async () => await repository.Get<Holiday, HolidayDto>());
    }

    /// <inheritdoc />
    public async Task<WorkedTimeInfoDto> GetWorkedDaysInfo(EntityFilter<TimeSheetDto> timeSheetFilter, EntityFilter<ProjectDto> projectFilter, EntityFilter<CustomerDto> customerFilter, EntityFilter<ActivityDto> activityFilter, EntityFilter<OrderDto> orderFilter, EntityFilter<HolidayDto> holidayFilter, CancellationToken cancellationToken = default)
    {
        var filter = FilterExtensions.CreateTimeSheetFilter(timeSheetFilter, projectFilter, customerFilter, activityFilter, orderFilter, holidayFilter);

        var workedTimes = await _repository
            .GetGrouped(
                groupBy: (TimeSheet x) => 1,
                select: x => new
                {
                    MinStartDate = x.Min(timeSheet => timeSheet.StartDateLocal),
                    MaxEndDate = x.Max(timeSheet => timeSheet.EndDateLocal),
                    WorkedTime = TimeSpan.FromSeconds(x.Sum(f => (double)f.StartDateLocal.DiffSeconds(f.StartDateOffset, f.EndDateLocal)))
                },
                where: filter,
                cancellationToken: cancellationToken
            );

        var workedTime = workedTimes.SingleOrDefault() ?? new
        {
            MinStartDate = DateTime.MinValue.AddDays(1),
            MaxEndDate = (DateTime?)DateTime.MaxValue.AddDays(-1),
            WorkedTime = TimeSpan.Zero
        };

        var selectedPeriod = FilterExtensions.GetSelectedPeriod(timeSheetFilter, true);

        var minStart = DateTimeOffset.MinValue.AddDays(1);
        var maxEnd = DateTimeOffset.MaxValue.AddDays(-1);
        var startDate = selectedPeriod.Start > minStart ? selectedPeriod.Start : workedTime.MinStartDate;
        var endDate = selectedPeriod.End < maxEnd ? selectedPeriod.End : workedTime.MaxEndDate!.Value;

        selectedPeriod = Section.Create(startDate, endDate);

        var settings = await _settingService.Get(cancellationToken);

        var workDays = await GetWorkdays(selectedPeriod, cancellationToken);
        return new WorkedTimeInfoDto
        {
            PublicWorkdays = workDays.PublicWorkdays.Count,
            PersonalWorkdays = workDays.PersonalWorkdays.Count,
            WorkHoursPerWorkday = settings.WorkHoursPerWorkday,
            WorkedTime = workedTime.WorkedTime,
        };
    }

    /// <inheritdoc />
    public async Task<WorkdaysDto> GetWorkdays(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
        => await GetWorkdays(startDate.GetDays(endDate), cancellationToken);

    /// <inheritdoc />
    public async Task<WorkdaysDto> GetWorkdays(Section<DateTimeOffset> dateTimeSection, CancellationToken cancellationToken = default)
        => dateTimeSection != null
            ? await GetWorkdays(dateTimeSection.Start.Date, dateTimeSection.End.Date, cancellationToken)
            : null;

    private async Task<WorkdaysDto> GetWorkdays(IEnumerable<DateTime> dates, CancellationToken cancellationToken = default)
    {
        var settings = await _settingService.Get(cancellationToken);
        var holidays = await _holidays;

        var workdays = settings.Workdays
            .Where(x => x.Value)
            .Select(x => x.Key)
            .ToList();

        var publicHolidayDates = holidays
            .Where(x => x.Type == HolidayType.PublicHoliday)
            .SelectMany(x => x.StartDate.Date.GetDays(x.EndDate.Date))
            .Distinct();

        var personalHolidayDates = holidays
            .Where(x => x.Type == HolidayType.Holiday)
            .SelectMany(x => x.StartDate.Date.GetDays(x.EndDate.Date))
            .Distinct();

        var publicWorkdays = dates
            .Where(date => workdays.Contains(date.DayOfWeek))
            .Except(publicHolidayDates)
            .ToList();

        var personalWorkdays = publicWorkdays
            .Except(personalHolidayDates)
            .ToList();

        return new WorkdaysDto
        {
            PublicWorkdays = publicWorkdays,
            PersonalWorkdays = personalWorkdays
        };
    }
}