using FS.FilterExpressionCreator.Abstractions.Models;
using FS.FilterExpressionCreator.Enums;
using FS.FilterExpressionCreator.Extensions;
using FS.FilterExpressionCreator.Filters;
using FS.TimeTracking.Abstractions.DTOs.MasterData;
using FS.TimeTracking.Abstractions.DTOs.TimeTracking;
using FS.TimeTracking.Abstractions.Extensions;
using FS.TimeTracking.Abstractions.Models.Application.MasterData;
using FS.TimeTracking.Abstractions.Models.Application.TimeTracking;
using System;

namespace FS.TimeTracking.Abstractions.Models.Application.Chart;

/// <summary>
/// Common filters used by chart services.
/// </summary>
public record struct ChartFilter(EntityFilter<TimeSheet> WorkedTimes, EntityFilter<Order> PlannedTimes, Range<DateTimeOffset> SelectedPeriod)
{
    /// <summary>
    /// The period selected by filter.
    /// </summary>
    public readonly Range<DateTimeOffset> SelectedPeriod = SelectedPeriod;

    /// <summary>
    /// The filter for worked times.
    /// </summary>
    public readonly EntityFilter<TimeSheet> WorkedTimes = WorkedTimes;

    /// <summary>
    /// The filter for planned times.
    /// </summary>
    public readonly EntityFilter<Order> PlannedTimes = PlannedTimes;

    internal static ChartFilter Create(EntityFilter<TimeSheetDto> timeSheetFilter, EntityFilter<ProjectDto> projectFilter, EntityFilter<CustomerDto> customerFilter, EntityFilter<ActivityDto> activityFilter, EntityFilter<OrderDto> orderFilter, EntityFilter<HolidayDto> holidayFilter)
    {
        var workedTimesFilter = FilterExtensions.CreateTimeSheetFilter(timeSheetFilter, projectFilter, customerFilter, activityFilter, orderFilter, holidayFilter);
        var plannedTimesFilter = FilterExtensions.CreateOrderFilter(timeSheetFilter, projectFilter, customerFilter, activityFilter, orderFilter, holidayFilter);
        var selectedPeriod = FilterExtensions.GetSelectedPeriod(timeSheetFilter);

        var start = selectedPeriod.Start;
        var end = selectedPeriod.End;
        if (start == DateTimeOffset.MinValue)
            start = DateTimeOffset.MinValue.AddDays(1); // Let space for timezone conversions.
        if (end == DateTimeOffset.MaxValue)
            end = DateTimeOffset.MaxValue.AddDays(-1); // Let space for timezone conversions.
        selectedPeriod = new(start, end);

        plannedTimesFilter = plannedTimesFilter
           .Replace(x => x.DueDate, FilterOperator.GreaterThanOrEqual, selectedPeriod.Start)
           .Replace(x => x.StartDate, FilterOperator.LessThan, selectedPeriod.End);

        return new ChartFilter(workedTimesFilter, plannedTimesFilter, selectedPeriod);
    }
}