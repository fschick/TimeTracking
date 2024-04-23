using FS.TimeTracking.Abstractions.DTOs.Administration;
using FS.TimeTracking.Core.Models.Application.Chart;
using FS.TimeTracking.Core.Models.Application.MasterData;
using FS.TimeTracking.Core.Models.Application.TimeTracking;
using FS.TimeTracking.Core.Models.Filter;
using Plainquire.Filter;
using System.Threading.Tasks;

namespace FS.TimeTracking.Core.Interfaces.Application.Services.Shared;

/// <summary>
/// Common filter creation service.
/// </summary>
public interface IFilterFactory
{
    /// <summary>
    /// Creates the filter for <see cref="Activity"/>.
    /// </summary>
    /// <param name="filters">Filters used to create result.</param>
    Task<EntityFilter<Activity>> CreateActivityFilter(TimeSheetFilterSet filters);

    /// <summary>
    /// Creates the filter for <see cref="Customer"/>.
    /// </summary>
    /// <param name="filters">Filters used to create result.</param>
    Task<EntityFilter<Customer>> CreateCustomerFilter(TimeSheetFilterSet filters);

    /// <summary>
    /// Creates the filter for <see cref="Order"/>.
    /// </summary>
    /// <param name="filters">Filters used to create result.</param>
    Task<EntityFilter<Order>> CreateOrderFilter(TimeSheetFilterSet filters);

    /// <summary>
    /// Creates the filter for <see cref="Order"/>.
    /// </summary>
    /// <param name="filters">Filters used to create result.</param>
    Task<EntityFilter<Project>> CreateProjectFilter(TimeSheetFilterSet filters);

    /// <summary>
    /// Creates the filter for <see cref="TimeSheet"/>.
    /// </summary>
    /// <param name="filters">Filters used to create result.</param>
    Task<EntityFilter<TimeSheet>> CreateTimeSheetFilter(TimeSheetFilterSet filters);

    /// <summary>
    /// Creates the filter for <see cref="Order"/>.
    /// </summary>
    /// <param name="filters">Filters used to create result.</param>
    Task<EntityFilter<Holiday>> CreateHolidayFilter(TimeSheetFilterSet filters);

    /// <summary>
    /// Creates the filter for <see cref="UserDto"/>.
    /// </summary>
    /// <param name="filters">Filters used to create result.</param>
    public Task<EntityFilter<UserDto>> CreateUserFilter(TimeSheetFilterSet filters);

    /// <summary>
    /// Creates a new chart filter.
    /// </summary>
    /// <param name="filters">Filters used to create result.</param>
    Task<ChartFilter> CreateChartFilter(TimeSheetFilterSet filters);
}