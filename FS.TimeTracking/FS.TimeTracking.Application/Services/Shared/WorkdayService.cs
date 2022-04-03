using FS.FilterExpressionCreator.Abstractions.Models;
using FS.FilterExpressionCreator.Extensions;
using FS.FilterExpressionCreator.Filters;
using FS.TimeTracking.Abstractions.DTOs.MasterData;
using FS.TimeTracking.Abstractions.DTOs.Shared;
using FS.TimeTracking.Abstractions.DTOs.TimeTracking;
using FS.TimeTracking.Abstractions.Enums;
using FS.TimeTracking.Abstractions.Extensions;
using FS.TimeTracking.Abstractions.Interfaces.Application.Services.MasterData;
using FS.TimeTracking.Abstractions.Interfaces.Application.Services.Shared;
using FS.TimeTracking.Abstractions.Interfaces.Repository.Services;
using FS.TimeTracking.Abstractions.Models.Application.MasterData;
using FS.TimeTracking.Abstractions.Models.Application.TimeTracking;
using FS.TimeTracking.Shared.Extensions;
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
    public async Task<WorkedDaysInfoDto> GetWorkedDaysInfo(EntityFilter<TimeSheetDto> timeSheetFilter, EntityFilter<ProjectDto> projectFilter, EntityFilter<CustomerDto> customerFilter, EntityFilter<ActivityDto> activityFilter, EntityFilter<OrderDto> orderFilter, EntityFilter<HolidayDto> holidayFilter, CancellationToken cancellationToken = default)
    {
        var filter = FilterExtensions.CreateTimeSheetFilter(timeSheetFilter, projectFilter, customerFilter, activityFilter, orderFilter, holidayFilter);

        var dbMinMaxDate = await _repository
            .GetGrouped(
                groupBy: (TimeSheet x) => 1,
                select: x => new
                {
                    Start = x.Min(timeSheet => timeSheet.StartDateLocal),
                    End = x.Max(timeSheet => timeSheet.EndDateLocal),
                    TotalWorkedTime = TimeSpan.FromSeconds(x.Sum(f => (double)f.StartDateLocal.DiffSeconds(f.StartDateOffset, f.EndDateLocal)))
                },
                where: filter,
                cancellationToken: cancellationToken
            )
            .AsEnumerableAsync()
            .FirstOrDefaultAsync();

        var now = DateTime.Now;
        var selectedPeriod = FilterExtensions.GetSelectedPeriod(timeSheetFilter, true);
        var selectionHasStartValue = selectedPeriod.Start != DateTimeOffset.MinValue;
        var selectionHasEndValue = selectedPeriod.End != DateTimeOffset.MaxValue;
        var startDate = selectionHasStartValue ? selectedPeriod.Start : dbMinMaxDate?.Start ?? now;
        var endDate = selectionHasEndValue ? selectedPeriod.End : dbMinMaxDate?.End ?? now;
        selectedPeriod = new Range<DateTimeOffset>(startDate, endDate);
        var workDays = await GetWorkdays(selectedPeriod, cancellationToken);

        var settings = await _settingService.GetSettings(cancellationToken);

        var totalWorkedTime = dbMinMaxDate?.TotalWorkedTime ?? TimeSpan.Zero;

        var (lastWorkedTimes, aggregationUnit) = dbMinMaxDate != null
            ? await GetLastWorkedTimes(dbMinMaxDate.Start, dbMinMaxDate.End, filter, cancellationToken)
            : (new(), WorkdayAggregationUnit.Invalid);

        return new WorkedDaysInfoDto
        {
            PublicWorkdays = workDays.PublicWorkdays.Count,
            PersonalWorkdays = workDays.PersonalWorkdays.Count,
            WorkHoursPerWorkday = settings.WorkHoursPerWorkday,
            TotalTimeWorked = totalWorkedTime,
            LastWorkedTimes = lastWorkedTimes.Take(7).ToList(),
            LastWorkedTimesAggregationUnit = aggregationUnit
        };
    }

    /// <inheritdoc />
    public async Task<WorkdaysDto> GetWorkdays(Range<DateTimeOffset> dateTimeRange, CancellationToken cancellationToken = default)
        => dateTimeRange != null
            ? await GetWorkdays(dateTimeRange.Start.Date, dateTimeRange.End.Date, cancellationToken)
            : null;

    /// <inheritdoc />
    public async Task<WorkdaysDto> GetWorkdays(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
        => startDate <= endDate
            ? await GetWorkdays(startDate.GetDays(endDate), cancellationToken)
            : new WorkdaysDto { PublicWorkdays = new(), PersonalWorkdays = new() };

    private async Task<WorkdaysDto> GetWorkdays(IEnumerable<DateTime> dates, CancellationToken cancellationToken = default)
    {
        var settings = await _settingService.GetSettings(cancellationToken);
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

    private async Task<(List<WorkdayDto>, WorkdayAggregationUnit)> GetLastWorkedTimes(DateTime minDate, DateTime? maxDate, EntityFilter<TimeSheet> filter, CancellationToken cancellationToken)
    {
        var start = minDate;
        var end = maxDate ?? DateTime.Now;

        var selectedDays = (end - start).TotalDays;
        var moreThan7Months = selectedDays > (6 * 30 + 28);
        var moreThan7Weeks = selectedDays > 49;
        var moreThan7Days = selectedDays > 7;
        const int atLeast7Years = 6 * 365 + 2 * 366;
        const int atLeast7Months = 4 * 31 + 3 * 30 + 1;
        const int atLeast7Weeks = 8 * 7 + 1;
        const int atLeast7Days = 7 + 1;

        WorkdayAggregationUnit aggregationUnit;
        if (moreThan7Months)
            aggregationUnit = WorkdayAggregationUnit.Year;
        else if (moreThan7Weeks)
            aggregationUnit = WorkdayAggregationUnit.Month;
        else if (moreThan7Days)
            aggregationUnit = WorkdayAggregationUnit.Week;
        else
            aggregationUnit = WorkdayAggregationUnit.Day;

        start = aggregationUnit switch
        {
            WorkdayAggregationUnit.Year => end.AddDays(-atLeast7Years),
            WorkdayAggregationUnit.Month => end.AddDays(-atLeast7Months),
            WorkdayAggregationUnit.Week => end.AddDays(-atLeast7Weeks),
            WorkdayAggregationUnit.Day => end.AddDays(-atLeast7Days),
            _ => throw new ArgumentOutOfRangeException(nameof(aggregationUnit))
        };

        filter = filter.Replace(x => x.StartDate, $"<{end:O}");
        filter = filter.Replace(x => x.EndDate, $">={start:O},ISNULL");

        var workedTimePerDay = await _repository
            .GetGrouped(
                groupBy: (TimeSheet x) => x.StartDateLocal.Date,
                select: x => new WorkdayDto
                {
                    Date = x.Min(timeSheet => timeSheet.StartDateLocal),
                    TimeWorked = TimeSpan.FromSeconds(x.Sum(f => (double)f.StartDateLocal.DiffSeconds(f.StartDateOffset, f.EndDateLocal)))
                },
                where: filter,
                cancellationToken: cancellationToken
            );

        var lastWorkedTimes = aggregationUnit switch
        {
            WorkdayAggregationUnit.Year => GroupAndFillMissing(workedTimePerDay, start, end, x => x.StartOfYear()),
            WorkdayAggregationUnit.Month => GroupAndFillMissing(workedTimePerDay, start, end, x => x.StartOfMonth()),
            WorkdayAggregationUnit.Week => GroupAndFillMissing(workedTimePerDay, start, end, x => x.StartOfWeek()),
            WorkdayAggregationUnit.Day => GroupAndFillMissing(workedTimePerDay, start, end, x => x.StartOfDay()),
            _ => throw new ArgumentOutOfRangeException(nameof(aggregationUnit))
        };

        return (lastWorkedTimes.Take(7).ToList(), aggregationUnit);
    }

    private static List<WorkdayDto> GroupAndFillMissing(IEnumerable<WorkdayDto> workedTimePerDay, DateTime start, DateTime end, Func<DateTime, DateTime> groupKeySelector)
    {
        var requiredDates = start.GetDays(end)
            .GroupBy(groupKeySelector)
            .TakeLast(7)
            .Select(x => x.Key)
            .ToList();

        var existingGroups = workedTimePerDay
            .GroupBy(workday => groupKeySelector(workday.Date))
            .Select(group => new WorkdayDto { Date = group.Key, TimeWorked = group.Sum(workday => workday.TimeWorked) })
            .ToList();

        return requiredDates
            .OuterJoin(
                existingGroups,
                requiredDate => requiredDate,
                workday => workday.Date,
                (requiredDate, workday) => workday ?? new() { Date = requiredDate, TimeWorked = TimeSpan.Zero }
            )
            .ToList();
    }

    private class WorkedTimePerDay
    {
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public TimeSpan WorkedTime { get; set; }
    }
}