using FS.FilterExpressionCreator.Filters;
using FS.TimeTracking.Abstractions.DTOs.Chart;
using FS.TimeTracking.Abstractions.DTOs.MasterData;
using FS.TimeTracking.Abstractions.DTOs.TimeTracking;
using FS.TimeTracking.Abstractions.Models.Application.Chart;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Abstractions.Interfaces.Application.Services.Chart;

/// <summary>
/// Order specific chart service
/// </summary>
public interface IOrderChartService
{
    /// <summary>
    /// Gets the work times grouped by order.
    /// </summary>
    /// <param name="timeSheetFilter">Filter applied to <see cref="TimeSheetDto"/>.</param>
    /// <param name="projectFilter">Filter applied to <see cref="ProjectDto"/>.</param>
    /// <param name="customerFilter">Filter applied to <see cref="CustomerDto"/>.</param>
    /// <param name="activityFilter">Filter applied to <see cref="ActivityDto"/>.</param>
    /// <param name="orderFilter">Filter applied to <see cref="OrderDto"/>.</param>
    /// <param name="holidayFilter">Filter applied to <see cref="HolidayDto"/>.</param>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    Task<List<OrderWorkTimeDto>> GetWorkTimesPerOrder(EntityFilter<TimeSheetDto> timeSheetFilter, EntityFilter<ProjectDto> projectFilter, EntityFilter<CustomerDto> customerFilter, EntityFilter<ActivityDto> activityFilter, EntityFilter<OrderDto> orderFilter, EntityFilter<HolidayDto> holidayFilter, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the planned times per order.
    /// </summary>
    /// <param name="filter">The filter to use.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    Task<List<OrderWorkTime>> GetPlannedTimesPerOrder(ChartFilter filter, CancellationToken cancellationToken);

    /// <summary>
    /// Gets the worked times per order.
    /// </summary>
    /// <param name="filter">The filter to use.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    Task<List<OrderWorkTime>> GetWorkedTimesPerOrder(ChartFilter filter, CancellationToken cancellationToken);

    /// <summary>
    /// Gets personal workdays count.
    /// </summary>
    /// <param name="startDate">The start date.</param>
    /// <param name="endDate">The end date.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    Task<int> GetPersonalWorkdaysCount(DateTime startDate, DateTime endDate, CancellationToken cancellationToken);
}