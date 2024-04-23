using FS.TimeTracking.Abstractions.DTOs.Administration;
using FS.TimeTracking.Abstractions.DTOs.MasterData;
using FS.TimeTracking.Abstractions.DTOs.TimeTracking;
using Plainquire.Filter;
using Plainquire.Filter.Abstractions;
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
    public EntityFilter<TimeSheetDto> TimeSheetFilter { get; set; } = new();

    /// <summary>
    /// Filter applied to<see cref="ProjectDto"/>.
    /// </summary>
    public EntityFilter<ProjectDto> ProjectFilter { get; set; } = new();

    /// <summary>
    /// Filter applied to<see cref="CustomerDto"/>.
    /// </summary>
    public EntityFilter<CustomerDto> CustomerFilter { get; set; } = new();

    /// <summary>
    /// Filter applied to<see cref="ActivityDto"/>.
    /// </summary>
    public EntityFilter<ActivityDto> ActivityFilter { get; set; } = new();

    /// <summary>
    /// Filter applied to<see cref="OrderDto"/>.
    /// </summary>
    public EntityFilter<OrderDto> OrderFilter { get; set; } = new();

    /// <summary>
    /// Filter applied to<see cref="HolidayDto"/>.
    /// </summary>
    public EntityFilter<HolidayDto> HolidayFilter { get; set; } = new();

    /// <summary>
    /// Filter applied to<see cref="UserDto"/>.
    /// </summary>
    public EntityFilter<UserDto> UserFilter { get; set; } = new();
}