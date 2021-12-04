using FS.FilterExpressionCreator.Filters;
using FS.TimeTracking.Api.REST.Routing;
using FS.TimeTracking.Shared.DTOs.MasterData;
using FS.TimeTracking.Shared.DTOs.Report;
using FS.TimeTracking.Shared.DTOs.TimeTracking;
using FS.TimeTracking.Shared.Interfaces.Application.Services.Report;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Api.REST.Controllers.Report;

/// <inheritdoc cref="IReportService" />
/// <seealso cref="ControllerBase" />
/// <seealso cref="IReportService" />
[V1ApiController]
public class ReportController : ControllerBase, IReportService
{
    private readonly IReportService _reportService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReportController"/> class.
    /// </summary>
    /// <param name="reportService">The report service.</param>
    public ReportController(IReportService reportService)
        => _reportService = reportService;

    /// <inheritdoc />
    [HttpGet]
    public async Task<List<WorkTimeDto>> GetWorkTimesPerCustomer([FromQuery] EntityFilter<TimeSheetDto> timeSheetFilter, [FromQuery] EntityFilter<ProjectDto> projectFilter, [FromQuery] EntityFilter<CustomerDto> customerFilter, [FromQuery] EntityFilter<ActivityDto> activityFilter, [FromQuery] EntityFilter<OrderDto> orderFilter, CancellationToken cancellationToken = default)
        => await _reportService.GetWorkTimesPerCustomer(timeSheetFilter, projectFilter, customerFilter, activityFilter, orderFilter, cancellationToken);

    /// <inheritdoc />
    [HttpGet]
    public async Task<List<WorkTimeDto>> GetWorkTimesPerOrder([FromQuery] EntityFilter<TimeSheetDto> timeSheetFilter, [FromQuery] EntityFilter<ProjectDto> projectFilter, [FromQuery] EntityFilter<CustomerDto> customerFilter, [FromQuery] EntityFilter<ActivityDto> activityFilter, [FromQuery] EntityFilter<OrderDto> orderFilter, CancellationToken cancellationToken = default)
        => await _reportService.GetWorkTimesPerOrder(timeSheetFilter, projectFilter, customerFilter, activityFilter, orderFilter, cancellationToken);
}