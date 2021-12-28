using FS.FilterExpressionCreator.Extensions;
using FS.FilterExpressionCreator.Filters;
using FS.FilterExpressionCreator.Models;
using FS.TimeTracking.Shared.DTOs.MasterData;
using FS.TimeTracking.Shared.DTOs.TimeTracking;
using FS.TimeTracking.Shared.Models.MasterData;
using FS.TimeTracking.Shared.Models.TimeTracking;
using System;
using System.Linq;

namespace FS.TimeTracking.Application.Extensions;

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
    public static Section<DateTimeOffset> GetSelectedPeriod(EntityFilter<TimeSheetDto> timeSheetFilter)
    {
        // Let space for later timezone conversions.
        var minValue = DateTimeOffset.MinValue.AddDays(1);
        var maxValue = DateTimeOffset.MaxValue.AddDays(-1);

        var startDateFilter = timeSheetFilter.GetPropertyFilter(x => x.StartDate);
        var endDateFilter = timeSheetFilter.GetPropertyFilter(x => x.EndDate);

        var startDate = endDateFilter != null
            ? ValueFilterExtensions.Create(endDateFilter).First().Value.ConvertStringToDateTimeOffset(DateTimeOffset.Now)
            : minValue;

        var endDate = startDateFilter != null
            ? ValueFilter.Create(startDateFilter).Value.ConvertStringToDateTimeOffset(DateTimeOffset.Now)
            : maxValue;

        return Section.Create(startDate, endDate);
    }
}