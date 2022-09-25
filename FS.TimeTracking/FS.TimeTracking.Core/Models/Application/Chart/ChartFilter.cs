using FS.FilterExpressionCreator.Abstractions.Models;
using FS.FilterExpressionCreator.Enums;
using FS.FilterExpressionCreator.Extensions;
using FS.FilterExpressionCreator.Filters;
using FS.TimeTracking.Core.Extensions;
using FS.TimeTracking.Core.Models.Application.MasterData;
using FS.TimeTracking.Core.Models.Application.TimeTracking;
using FS.TimeTracking.Core.Models.Filter;
using System;

namespace FS.TimeTracking.Core.Models.Application.Chart;

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

    /// <summary>
    /// Creates a new chart filter.
    /// </summary>
    /// <param name="filters">Filters used to create result.</param>
    public static ChartFilter Create(TimeSheetFilterSet filters)
    {
        var workedTimesFilter = FilterExtensions.CreateTimeSheetFilter(filters);
        var plannedTimesFilter = FilterExtensions.CreateOrderFilter(filters);
        var selectedPeriod = FilterExtensions.GetSelectedPeriod(filters);

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