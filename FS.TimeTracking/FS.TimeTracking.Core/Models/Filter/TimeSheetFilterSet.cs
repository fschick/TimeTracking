using FS.FilterExpressionCreator.Abstractions.Attributes;
using FS.FilterExpressionCreator.Filters;
using FS.TimeTracking.Abstractions.DTOs.MasterData;
using FS.TimeTracking.Abstractions.DTOs.TimeTracking;
using System.Diagnostics.CodeAnalysis;

namespace FS.TimeTracking.Core.Models.Filter;

/// <summary>
/// Common set of filters used by most API operations.
/// </summary>
[EntityFilterSet]
[ExcludeFromCodeCoverage]
public class TimeSheetFilterSet
{
    /// <summary>
    /// Filter applied to<see cref="TimeSheetDto"/>.
    /// </summary>
    public EntityFilter<TimeSheetDto> TimeSheetFilter { get; set; }

    /// <summary>
    /// Filter applied to<see cref="ProjectDto"/>.
    /// </summary>
    public EntityFilter<ProjectDto> ProjectFilter { get; set; }

    /// <summary>
    /// Filter applied to<see cref="CustomerDto"/>.
    /// </summary>
    public EntityFilter<CustomerDto> CustomerFilter { get; set; }

    /// <summary>
    /// Filter applied to<see cref="ActivityDto"/>.
    /// </summary>
    public EntityFilter<ActivityDto> ActivityFilter { get; set; }

    /// <summary>
    /// Filter applied to<see cref="OrderDto"/>.
    /// </summary>
    public EntityFilter<OrderDto> OrderFilter { get; set; }

    /// <summary>
    /// Filter applied to<see cref="HolidayDto"/>.
    /// </summary>
    public EntityFilter<HolidayDto> HolidayFilter { get; set; }
}