using FS.FilterExpressionCreator.Abstractions.Models;
using FS.FilterExpressionCreator.Filters;
using FS.TimeTracking.Core.Models.Application.Chart;
using FS.TimeTracking.Core.Models.Application.MasterData;
using FS.TimeTracking.Core.Models.Application.TimeTracking;
using FS.TimeTracking.Core.Models.Filter;
using System;
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
    /// Creates a new chart filter.
    /// </summary>
    /// <param name="filters">Filters used to create result.</param>
    Task<ChartFilter> CreateChartFilter(TimeSheetFilterSet filters);

    /// <summary>
    /// Gets the selection period from <paramref name="filters.TimeSheetFilter"/>.
    /// </summary>
    /// <param name="filters">Filters used to create result.</param>
    /// <param name="endDateExclusive">Handle given end date as (right) exclusive.</param>
    Task<Range<DateTimeOffset>> GetSelectedPeriod(TimeSheetFilterSet filters, bool endDateExclusive = false);

    /// <summary>
    /// Creates HTTP query parameters from given filters.
    /// </summary>
    /// <param name="filters">Filters used to create result.</param>
    /// <param name="additionalParameters">A variable-length parameters list containing additional parameters.</param>
    Task<string> ToQueryParams(TimeSheetFilterSet filters, params (string key, string value)[] additionalParameters);
}