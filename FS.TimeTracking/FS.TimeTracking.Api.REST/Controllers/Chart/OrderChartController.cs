using FS.FilterExpressionCreator.Filters;
using FS.TimeTracking.Abstractions.DTOs.Chart;
using FS.TimeTracking.Abstractions.DTOs.MasterData;
using FS.TimeTracking.Abstractions.DTOs.TimeTracking;
using FS.TimeTracking.Abstractions.Interfaces.Application.Services.Chart;
using FS.TimeTracking.Abstractions.Models.Application.Chart;
using FS.TimeTracking.Api.REST.Routing;
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
}