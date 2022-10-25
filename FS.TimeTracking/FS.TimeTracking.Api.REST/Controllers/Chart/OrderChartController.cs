using FS.TimeTracking.Abstractions.Constants;
using FS.TimeTracking.Abstractions.DTOs.Chart;
using FS.TimeTracking.Api.REST.Routing;
using FS.TimeTracking.Core.Interfaces.Application.Services.Chart;
using FS.TimeTracking.Core.Models.Application.Chart;
using FS.TimeTracking.Core.Models.Filter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Api.REST.Controllers.Chart;

/// <inheritdoc cref="IOrderChartService" />
/// <seealso cref="ControllerBase" />
/// <seealso cref="IOrderChartService" />
[ApiV1Controller]
[Authorize(Roles = RoleNames.CHARTS_BY_ORDER_VIEW)]
[ExcludeFromCodeCoverage]
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
    public async Task<List<OrderWorkTimeDto>> GetWorkTimesPerOrder([FromQuery] TimeSheetFilterSet filters, CancellationToken cancellationToken = default)
        => await _chartService.GetWorkTimesPerOrder(filters, cancellationToken);

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