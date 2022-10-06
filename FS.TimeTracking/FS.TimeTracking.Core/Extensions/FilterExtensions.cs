using FS.FilterExpressionCreator.Abstractions.Models;
using FS.FilterExpressionCreator.Extensions;
using FS.FilterExpressionCreator.Filters;
using FS.TimeTracking.Core.Constants;
using FS.TimeTracking.Core.Models.Application.MasterData;
using FS.TimeTracking.Core.Models.Application.TimeTracking;
using FS.TimeTracking.Core.Models.Filter;
using System;
using System.Linq;
using System.Web;

namespace FS.TimeTracking.Core.Extensions;

/// <summary>
/// Extensions to create filters.
/// </summary>
public static class FilterExtensions
{
    /// <summary>
    /// Creates the filter for <see cref="Activity"/>.
    /// </summary>
    /// <param name="filters">Filters used to create result.</param>
    public static EntityFilter<Activity> CreateActivityFilter(TimeSheetFilterSet filters)
    {
        var customerFilter = filters.CustomerFilter
            .Cast<Customer>()
            .Clear(x => x.Id);

        var projectFilter = filters.ProjectFilter
            .Cast<Project>()
            .Clear(x => x.Id);

        var filter = filters.ActivityFilter
            .Cast<Activity>()
            .Replace(x => x.CustomerId, filters.CustomerFilter.GetPropertyFilter(p => p.Id))
            .Replace(x => x.Customer, customerFilter)
            .Replace(x => x.ProjectId, filters.ProjectFilter.GetPropertyFilter(p => p.Id))
            .Replace(x => x.Project, projectFilter);

        return filter;
    }

    /// <summary>
    /// Creates the filter for <see cref="Customer"/>.
    /// </summary>
    /// <param name="filters">Filters used to create result.</param>
    public static EntityFilter<Customer> CreateCustomerFilter(TimeSheetFilterSet filters)
    {
        var filter = filters.CustomerFilter
            .Cast<Customer>();
        return filter;
    }

    /// <summary>
    /// Creates the filter for <see cref="Order"/>.
    /// </summary>
    /// <param name="filters">Filters used to create result.</param>
    public static EntityFilter<Order> CreateOrderFilter(TimeSheetFilterSet filters)
    {
        var customerProjectFilter = filters.CustomerFilter
            .Cast<Customer>()
            .Clear(x => x.Id)
            .Replace(x => x.Projects, filters.ProjectFilter.Cast<Project>());

        var filter = filters.OrderFilter
            .Cast<Order>()
            .Replace(x => x.CustomerId, filters.CustomerFilter.GetPropertyFilter(c => c.Id))
            .Replace(x => x.Customer, customerProjectFilter);

        return filter;
    }

    /// <summary>
    /// Creates the filter for <see cref="Order"/>.
    /// </summary>
    /// <param name="filters">Filters used to create result.</param>
    public static EntityFilter<Project> CreateProjectFilter(TimeSheetFilterSet filters)
    {
        var customerFilter = filters.CustomerFilter
            .Cast<Customer>()
            .Clear(x => x.Id);

        var filter = filters.ProjectFilter
            .Cast<Project>()
            .Replace(x => x.CustomerId, filters.CustomerFilter.GetPropertyFilter(c => c.Id))
            .Replace(x => x.Customer, customerFilter);

        return filter;
    }

    /// <summary>
    /// Creates the filter for <see cref="TimeSheet"/>.
    /// </summary>
    /// <param name="filters">Filters used to create result.</param>
    public static EntityFilter<TimeSheet> CreateTimeSheetFilter(TimeSheetFilterSet filters)
    {
        var customerFilter = filters.CustomerFilter
            .Cast<Customer>()
            .Clear(x => x.Id);

        var activityFilter = filters.ActivityFilter
            .Cast<Activity>()
            .Clear(x => x.Id);

        var projectFilter = filters.ProjectFilter
            .Cast<Project>()
            .Clear(x => x.Id);

        var orderFilter = filters.OrderFilter
            .Cast<Order>()
            .Clear(x => x.Id);

        var filter = filters.TimeSheetFilter
            .Cast<TimeSheet>()
            .Replace(x => x.CustomerId, filters.CustomerFilter.GetPropertyFilter(p => p.Id))
            .Replace(x => x.Customer, customerFilter)
            .Replace(x => x.ActivityId, filters.ActivityFilter.GetPropertyFilter(a => a.Id))
            .Replace(x => x.Activity, activityFilter)
            .Replace(x => x.ProjectId, filters.ProjectFilter.GetPropertyFilter(p => p.Id))
            .Replace(x => x.Project, projectFilter)
            .Replace(x => x.OrderId, filters.OrderFilter.GetPropertyFilter(o => o.Id))
            .Replace(x => x.Order, orderFilter);

        return filter;
    }

    /// <summary>
    /// Creates the filter for <see cref="Order"/>.
    /// </summary>
    /// <param name="filters">Filters used to create result.</param>
    public static EntityFilter<Holiday> CreateHolidayFilter(TimeSheetFilterSet filters)
    {
        var filter = filters.HolidayFilter
            .Cast<Holiday>();

        return filter;
    }

    /// <summary>
    /// Gets the selection period from <paramref name="filters.TimeSheetFilter"/>.
    /// </summary>
    /// <param name="filters">Filters used to create result.</param>
    /// <param name="endDateExclusive">Handle given end date as (right) exclusive.</param>
    public static Range<DateTimeOffset> GetSelectedPeriod(TimeSheetFilterSet filters, bool endDateExclusive = false)
    {
        var startDateFilter = filters.TimeSheetFilter.GetPropertyFilter(x => x.StartDate);
        var endDateFilter = filters.TimeSheetFilter.GetPropertyFilter(x => x.EndDate);

        var startDate = endDateFilter != null
            ? ValueFilterExtensions.Create(endDateFilter).First().Value.ConvertStringToDateTimeOffset(DateTimeOffset.Now)
            : DateOffset.MinDate;

        var endDate = startDateFilter != null
            ? ValueFilter.Create(startDateFilter).Value.ConvertStringToDateTimeOffset(DateTimeOffset.Now)
            : DateOffset.MaxDate; // Let space for timezone conversions.

        if (endDate != DateOffset.MaxDate && endDateExclusive)
            endDate = endDate.AddDays(-1);

        return new Range<DateTimeOffset>(startDate, endDate);
    }

    /// <summary>
    /// Creates HTTP query parameters from given filters.
    /// </summary>
    /// <param name="filters">Filters used to create result.</param>
    /// <param name="additionalParameters">A variable-length parameters list containing additional parameters.</param>
    public static string ToQueryParams(TimeSheetFilterSet filters, params (string key, string value)[] additionalParameters)
    {
        var filterParameters = new[]
        {
            filters.TimeSheetFilter.ToQueryParams(),
            filters.ProjectFilter.ToQueryParams(),
            filters.CustomerFilter.ToQueryParams(),
            filters.ActivityFilter.ToQueryParams(),
            filters.OrderFilter.ToQueryParams(),
            filters.HolidayFilter.ToQueryParams(),
        };

        var additionalParams = additionalParameters.Select(param => $"{HttpUtility.UrlEncode(param.key)}={HttpUtility.UrlEncode(param.value)}");
        var keyValuePairs = filterParameters.Concat(additionalParams).Where(x => !string.IsNullOrWhiteSpace(x));
        return string.Join('&', keyValuePairs);
    }
}