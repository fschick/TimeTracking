using FS.FilterExpressionCreator.Filters;
using FS.TimeTracking.Abstractions.DTOs.Chart;
using FS.TimeTracking.Abstractions.DTOs.MasterData;
using FS.TimeTracking.Abstractions.DTOs.TimeTracking;
using FS.TimeTracking.Api.REST.Routing;
using FS.TimeTracking.Core.Interfaces.Application.Services.Chart;
using FS.TimeTracking.Core.Models.Application.Chart;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Api.REST.Controllers.Chart;

/// <inheritdoc cref="IOrderChartService" />
/// <seealso cref="ControllerBase" />
/// <seealso cref="IOrderChartService" />
[V1ApiController]
public class OrderChartController : ControllerBase, IOrderChartService
{
    private readonly IOrderChartService _chartService;

    /// <summary>
    /// Initializes a new instance of the <see cref="OrderChartController"/> class.
    /// </summary>
    /// <param name="chartService">The chart service.</param>
    public OrderChartController(IOrderChartService chartService)
        => _chartService = chartService;

    /// <inheritdoc />
    [HttpGet]
    public async Task<List<OrderWorkTimeDto>> GetWorkTimesPerOrder([FromQuery] EntityFilter<TimeSheetDto> timeSheetFilter, [FromQuery] EntityFilter<ProjectDto> projectFilter, [FromQuery] EntityFilter<CustomerDto> customerFilter, [FromQuery] EntityFilter<ActivityDto> activityFilter, [FromQuery] EntityFilter<OrderDto> orderFilter, [FromQuery] EntityFilter<HolidayDto> holidayFilter, CancellationToken cancellationToken = default)
        => await _chartService.GetWorkTimesPerOrder(timeSheetFilter, projectFilter, customerFilter, activityFilter, orderFilter, holidayFilter, cancellationToken);

    [NonAction]
    Task<List<OrderWorkTime>> IOrderChartService.GetPlannedTimesPerOrder(ChartFilter filter, CancellationToken cancellationToken)
        => throw new NotImplementedException("For internal usage only");

    [NonAction]
    Task<List<OrderWorkTime>> IOrderChartService.GetWorkedTimesPerOrder(ChartFilter filter, CancellationToken cancellationToken)
        => throw new NotImplementedException("For internal usage only");

    /// <inheritdoc />
    [HttpGet]
    public async Task<int> GetPersonalWorkdaysCount(DateTimeOffset startDate, DateTimeOffset endDate, CancellationToken cancellationToken)
        => await _chartService.GetPersonalWorkdaysCount(startDate, endDate, cancellationToken);
}