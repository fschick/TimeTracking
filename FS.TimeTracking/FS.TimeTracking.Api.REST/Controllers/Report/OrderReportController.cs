using FS.FilterExpressionCreator.Filters;
using FS.TimeTracking.Api.REST.Routing;
using FS.TimeTracking.Shared.DTOs.MasterData;
using FS.TimeTracking.Shared.DTOs.Report;
using FS.TimeTracking.Shared.DTOs.TimeTracking;
using FS.TimeTracking.Shared.Interfaces.Application.Services.Report;
using FS.TimeTracking.Shared.Models.Application.Report;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Api.REST.Controllers.Report;

/// <inheritdoc cref="IOrderReportService" />
/// <seealso cref="ControllerBase" />
/// <seealso cref="IOrderReportService" />
[V1ApiController]
public class OrderReportController : ControllerBase, IOrderReportService
{
    private readonly IOrderReportService _reportService;

    /// <summary>
    /// Initializes a new instance of the <see cref="OrderReportController"/> class.
    /// </summary>
    /// <param name="reportService">The report service.</param>
    public OrderReportController(IOrderReportService reportService)
        => _reportService = reportService;

    /// <inheritdoc />
    [HttpGet]
    public async Task<List<OrderWorkTimeDto>> GetWorkTimesPerOrder([FromQuery] EntityFilter<TimeSheetDto> timeSheetFilter, [FromQuery] EntityFilter<ProjectDto> projectFilter, [FromQuery] EntityFilter<CustomerDto> customerFilter, [FromQuery] EntityFilter<ActivityDto> activityFilter, [FromQuery] EntityFilter<OrderDto> orderFilter, [FromQuery] EntityFilter<HolidayDto> holidayFilter, CancellationToken cancellationToken = default)
        => await _reportService.GetWorkTimesPerOrder(timeSheetFilter, projectFilter, customerFilter, activityFilter, orderFilter, holidayFilter, cancellationToken);

    [NonAction]
    Task<List<OrderWorkTime>> IOrderReportService.GetPlannedTimesPerOrder(ReportServiceFilter filter, CancellationToken cancellationToken)
        => throw new NotImplementedException("For internal usage only");

    [NonAction]
    Task<List<OrderWorkTime>> IOrderReportService.GetWorkedTimesPerOrder(ReportServiceFilter filter, CancellationToken cancellationToken)
        => throw new NotImplementedException("For internal usage only");
}