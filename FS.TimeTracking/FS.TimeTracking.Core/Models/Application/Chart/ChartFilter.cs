using FS.FilterExpressionCreator.Abstractions.Models;
using FS.FilterExpressionCreator.Filters;
using FS.TimeTracking.Core.Models.Application.MasterData;
using FS.TimeTracking.Core.Models.Application.TimeTracking;
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
}