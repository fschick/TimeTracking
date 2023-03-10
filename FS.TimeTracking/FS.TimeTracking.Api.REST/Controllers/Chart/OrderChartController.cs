using FS.TimeTracking.Abstractions.Constants;
using FS.TimeTracking.Abstractions.DTOs.Chart;
using FS.TimeTracking.Api.REST.Routing;
using FS.TimeTracking.Core.Interfaces.Application.Services.Chart;
using FS.TimeTracking.Core.Models.Filter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Api.REST.Controllers.Chart;

/// <inheritdoc cref="IOrderChartApiService" />
/// <seealso cref="ControllerBase" />
/// <seealso cref="IOrderChartApiService" />
[ApiV1Controller]
[Authorize(Roles = RoleName.CHARTS_BY_ORDER_VIEW)]
[ExcludeFromCodeCoverage]
public class OrderChartController : ControllerBase, IOrderChartApiService
{
    private readonly IOrderChartApiService _chartService;

    /// <summary>
    /// Initializes a new instance of the <see cref="OrderChartController"/> class.
    /// </summary>
    /// <param name="chartService">The chart service.</param>
    public OrderChartController(IOrderChartApiService chartService)
        => _chartService = chartService;

    /// <inheritdoc />
    [HttpGet]
    public async Task<List<OrderWorkTimeDto>> GetWorkTimesPerOrder([FromQuery] TimeSheetFilterSet filters, CancellationToken cancellationToken = default)
        => await _chartService.GetWorkTimesPerOrder(filters, cancellationToken);

    /// <inheritdoc />
    [HttpGet]
    public async Task<int> GetPersonalWorkdaysCount(TimeSheetFilterSet filters, DateTimeOffset startDate, DateTimeOffset endDate, CancellationToken cancellationToken)
        => await _chartService.GetPersonalWorkdaysCount(filters, startDate, endDate, cancellationToken);
}