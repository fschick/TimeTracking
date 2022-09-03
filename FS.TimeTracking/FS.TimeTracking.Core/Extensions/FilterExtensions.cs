using FS.FilterExpressionCreator.Abstractions.Models;
using FS.FilterExpressionCreator.Extensions;
using FS.FilterExpressionCreator.Filters;
using FS.TimeTracking.Abstractions.DTOs.MasterData;
using FS.TimeTracking.Abstractions.DTOs.TimeTracking;
using FS.TimeTracking.Core.Models.Application.MasterData;
using FS.TimeTracking.Core.Models.Application.TimeTracking;
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
    /// <param name="timeSheetFilter">Filter applied to <see cref="TimeSheetDto"/>.</param>
    /// <param name="projectFilter">Filter applied to <see cref="ProjectDto"/>.</param>
    /// <param name="customerFilter">Filter applied to <see cref="CustomerDto"/>.</param>
    /// <param name="activityFilter">Filter applied to <see cref="ActivityDto"/>.</param>
    /// <param name="orderFilter">Filter applied to <see cref="OrderDto"/>.</param>
    /// <param name="holidayFilter">Filter applied to <see cref="HolidayDto"/>.</param>
    public static EntityFilter<Activity> CreateActivityFilter(EntityFilter<TimeSheetDto> timeSheetFilter, EntityFilter<ProjectDto> projectFilter, EntityFilter<CustomerDto> customerFilter, EntityFilter<ActivityDto> activityFilter, EntityFilter<OrderDto> orderFilter, EntityFilter<HolidayDto> holidayFilter)
    {
        var customerFilter1 = customerFilter
            .Cast<Customer>()
            .Clear(x => x.Id);

        var projectCustomerFilter = projectFilter
            .Cast<Project>()
            .Clear(x => x.Id)
            .Replace(x => x.CustomerId, customerFilter.GetPropertyFilter(c => c.Id))
            .Replace(x => x.Customer, customerFilter1);

        var filter = activityFilter
            .Cast<Activity>()
            .Replace(x => x.ProjectId, projectFilter.GetPropertyFilter(p => p.Id))
            .Replace(x => x.Project, projectCustomerFilter);

        return filter;
    }

    /// <summary>
    /// Creates the filter for <see cref="Customer"/>.
    /// </summary>
    /// <param name="timeSheetFilter">Filter applied to <see cref="TimeSheetDto"/>.</param>
    /// <param name="projectFilter">Filter applied to <see cref="ProjectDto"/>.</param>
    /// <param name="customerFilter">Filter applied to <see cref="CustomerDto"/>.</param>
    /// <param name="activityFilter">Filter applied to <see cref="ActivityDto"/>.</param>
    /// <param name="orderFilter">Filter applied to <see cref="OrderDto"/>.</param>
    /// <param name="holidayFilter">Filter applied to <see cref="HolidayDto"/>.</param>
    public static EntityFilter<Customer> CreateCustomerFilter(EntityFilter<TimeSheetDto> timeSheetFilter, EntityFilter<ProjectDto> projectFilter, EntityFilter<CustomerDto> customerFilter, EntityFilter<ActivityDto> activityFilter, EntityFilter<OrderDto> orderFilter, EntityFilter<HolidayDto> holidayFilter)
    {
        var filter = customerFilter
            .Cast<Customer>();
        return filter;
    }

    /// <summary>
    /// Creates the filter for <see cref="Order"/>.
    /// </summary>
    /// <param name="timeSheetFilter">Filter applied to <see cref="TimeSheetDto"/>.</param>
    /// <param name="projectFilter">Filter applied to <see cref="ProjectDto"/>.</param>
    /// <param name="customerFilter">Filter applied to <see cref="CustomerDto"/>.</param>
    /// <param name="activityFilter">Filter applied to <see cref="ActivityDto"/>.</param>
    /// <param name="orderFilter">Filter applied to <see cref="OrderDto"/>.</param>
    /// <param name="holidayFilter">Filter applied to <see cref="HolidayDto"/>.</param>
    public static EntityFilter<Order> CreateOrderFilter(EntityFilter<TimeSheetDto> timeSheetFilter, EntityFilter<ProjectDto> projectFilter, EntityFilter<CustomerDto> customerFilter, EntityFilter<ActivityDto> activityFilter, EntityFilter<OrderDto> orderFilter, EntityFilter<HolidayDto> holidayFilter)
    {
        var customerProjectFilter = customerFilter
            .Cast<Customer>()
            .Clear(x => x.Id)
            .Replace(x => x.Projects, projectFilter.Cast<Project>());

        var filter = orderFilter
            .Cast<Order>()
            .Replace(x => x.CustomerId, customerFilter.GetPropertyFilter(c => c.Id))
            .Replace(x => x.Customer, customerProjectFilter);

        return filter;
    }

    /// <summary>
    /// Creates the filter for <see cref="Order"/>.
    /// </summary>
    /// <param name="timeSheetFilter">Filter applied to <see cref="TimeSheetDto"/>.</param>
    /// <param name="projectFilter">Filter applied to <see cref="ProjectDto"/>.</param>
    /// <param name="customerFilter">Filter applied to <see cref="CustomerDto"/>.</param>
    /// <param name="activityFilter">Filter applied to <see cref="ActivityDto"/>.</param>
    /// <param name="orderFilter">Filter applied to <see cref="OrderDto"/>.</param>
    /// <param name="holidayFilter">Filter applied to <see cref="HolidayDto"/>.</param>
    public static EntityFilter<Project> CreateProjectFilter(EntityFilter<TimeSheetDto> timeSheetFilter, EntityFilter<ProjectDto> projectFilter, EntityFilter<CustomerDto> customerFilter, EntityFilter<ActivityDto> activityFilter, EntityFilter<OrderDto> orderFilter, EntityFilter<HolidayDto> holidayFilter)
    {
        var customerFilter1 = customerFilter
            .Cast<Customer>()
            .Clear(x => x.Id);

        var filter = projectFilter
            .Cast<Project>()
            .Replace(x => x.CustomerId, customerFilter.GetPropertyFilter(c => c.Id))
            .Replace(x => x.Customer, customerFilter1);

        return filter;
    }

    /// <summary>
    /// Creates the filter for <see cref="TimeSheet"/>.
    /// </summary>
    /// <param name="timeSheetFilter">Filter applied to <see cref="TimeSheetDto"/>.</param>
    /// <param name="projectFilter">Filter applied to <see cref="ProjectDto"/>.</param>
    /// <param name="customerFilter">Filter applied to <see cref="CustomerDto"/>.</param>
    /// <param name="activityFilter">Filter applied to <see cref="ActivityDto"/>.</param>
    /// <param name="orderFilter">Filter applied to <see cref="OrderDto"/>.</param>
    /// <param name="holidayFilter">Filter applied to <see cref="HolidayDto"/>.</param>
    public static EntityFilter<TimeSheet> CreateTimeSheetFilter(EntityFilter<TimeSheetDto> timeSheetFilter, EntityFilter<ProjectDto> projectFilter, EntityFilter<CustomerDto> customerFilter, EntityFilter<ActivityDto> activityFilter, EntityFilter<OrderDto> orderFilter, EntityFilter<HolidayDto> holidayFilter)
    {
        var customerFilter1 = customerFilter
            .Cast<Customer>()
            .Clear(x => x.Id);

        var projectCustomerFilter = projectFilter
            .Cast<Project>()
            .Clear(x => x.Id)
            .Replace(x => x.CustomerId, customerFilter.GetPropertyFilter(c => c.Id))
            .Replace(x => x.Customer, customerFilter1);

        var activityFilter1 = activityFilter
            .Cast<Activity>()
            .Clear(x => x.Id);

        var orderFilter1 = orderFilter
            .Cast<Order>()
            .Clear(x => x.Id);

        var filter = timeSheetFilter
            .Cast<TimeSheet>()
            .Replace(x => x.ProjectId, projectFilter.GetPropertyFilter(p => p.Id))
            .Replace(x => x.Project, projectCustomerFilter)
            .Replace(x => x.ActivityId, activityFilter.GetPropertyFilter(a => a.Id))
            .Replace(x => x.Activity, activityFilter1)
            .Replace(x => x.OrderId, orderFilter.GetPropertyFilter(o => o.Id))
            .Replace(x => x.Order, orderFilter1);

        return filter;
    }

    /// <summary>
    /// Creates the filter for <see cref="Order"/>.
    /// </summary>
    /// <param name="timeSheetFilter">Filter applied to <see cref="TimeSheetDto"/>.</param>
    /// <param name="projectFilter">Filter applied to <see cref="ProjectDto"/>.</param>
    /// <param name="customerFilter">Filter applied to <see cref="CustomerDto"/>.</param>
    /// <param name="activityFilter">Filter applied to <see cref="ActivityDto"/>.</param>
    /// <param name="orderFilter">Filter applied to <see cref="OrderDto"/>.</param>
    /// <param name="holidayFilter">Filter applied to <see cref="HolidayDto"/>.</param>
    public static EntityFilter<Holiday> CreateHolidayFilter(EntityFilter<TimeSheetDto> timeSheetFilter, EntityFilter<ProjectDto> projectFilter, EntityFilter<CustomerDto> customerFilter, EntityFilter<ActivityDto> activityFilter, EntityFilter<OrderDto> orderFilter, EntityFilter<HolidayDto> holidayFilter)
    {
        var filter = holidayFilter
            .Cast<Holiday>();

        return filter;
    }

    /// <summary>
    /// Gets the selection period from <paramref name="timeSheetFilter"/>.
    /// </summary>
    /// <param name="timeSheetFilter">The time sheet filter.</param>
    /// <param name="endDateExclusive">Handle given end date as (right) exclusive.</param>
    public static Range<DateTimeOffset> GetSelectedPeriod(EntityFilter<TimeSheetDto> timeSheetFilter, bool endDateExclusive = false)
    {
        var startDateFilter = timeSheetFilter.GetPropertyFilter(x => x.StartDate);
        var endDateFilter = timeSheetFilter.GetPropertyFilter(x => x.EndDate);

        var startDate = endDateFilter != null
            ? ValueFilterExtensions.Create(endDateFilter).First().Value.ConvertStringToDateTimeOffset(DateTimeOffset.Now)
            : DateTimeOffset.MinValue;

        var endDate = startDateFilter != null
            ? ValueFilter.Create(startDateFilter).Value.ConvertStringToDateTimeOffset(DateTimeOffset.Now)
            : DateTimeOffset.MaxValue;

        if (endDate != DateTimeOffset.MaxValue && endDateExclusive)
            endDate = endDate.AddDays(-1);

        return new Range<DateTimeOffset>(startDate, endDate);
    }

    /// <summary>
    /// Creates HTTP query parameters from given filters.
    /// </summary>
    /// <param name="timeSheetFilter">Filter applied to <see cref="TimeSheetDto"/>.</param>
    /// <param name="projectFilter">Filter applied to <see cref="ProjectDto"/>.</param>
    /// <param name="customerFilter">Filter applied to <see cref="CustomerDto"/>.</param>
    /// <param name="activityFilter">Filter applied to <see cref="ActivityDto"/>.</param>
    /// <param name="orderFilter">Filter applied to <see cref="OrderDto"/>.</param>
    /// <param name="holidayFilter">Filter applied to <see cref="HolidayDto"/>.</param>
    public static string ToQueryParams(EntityFilter<TimeSheetDto> timeSheetFilter, EntityFilter<ProjectDto> projectFilter, EntityFilter<CustomerDto> customerFilter, EntityFilter<ActivityDto> activityFilter, EntityFilter<OrderDto> orderFilter, EntityFilter<HolidayDto> holidayFilter, params (string key, string value)[] additionalParameters)
    {
        var filterParameters = new[]
        {
            timeSheetFilter.ToQueryParams(),
            projectFilter.ToQueryParams(),
            customerFilter.ToQueryParams(),
            activityFilter.ToQueryParams(),
            orderFilter.ToQueryParams(),
            holidayFilter.ToQueryParams(),
        };

        var additionalParams = additionalParameters.Select(param => $"{HttpUtility.UrlEncode(param.key)}={HttpUtility.UrlEncode(param.value)}");
        var keyValuePairs = filterParameters.Concat(additionalParams).Where(x => !string.IsNullOrWhiteSpace(x));
        return string.Join('&', keyValuePairs);
    }
}