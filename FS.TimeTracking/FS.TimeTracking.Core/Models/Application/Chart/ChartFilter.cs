using FS.FilterExpressionCreator.Abstractions.Models;
using FS.FilterExpressionCreator.Enums;
using FS.FilterExpressionCreator.Extensions;
using FS.FilterExpressionCreator.Filters;
using FS.TimeTracking.Core.Constants;
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

        var selectedPeriodForFilter = FilterExtensions.GetSelectedPeriod(filters);

        if (selectedPeriodForFilter.Start != DateOffset.MinDate)
            plannedTimesFilter = plannedTimesFilter
               .Replace(x => x.DueDate, FilterOperator.GreaterThanOrEqual, selectedPeriodForFilter.Start);

        if (selectedPeriodForFilter.End != DateOffset.MaxDate)
            plannedTimesFilter = plannedTimesFilter
                .Replace(x => x.StartDate, FilterOperator.LessThan, selectedPeriodForFilter.End);

        var selectedPeriod = FilterExtensions.GetSelectedPeriod(filters, true);
        return new ChartFilter(workedTimesFilter, plannedTimesFilter, selectedPeriod);
    }
}