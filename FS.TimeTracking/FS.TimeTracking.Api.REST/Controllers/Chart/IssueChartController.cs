using FS.FilterExpressionCreator.Filters;
using FS.TimeTracking.Abstractions.DTOs.Chart;
using FS.TimeTracking.Abstractions.DTOs.MasterData;
using FS.TimeTracking.Abstractions.DTOs.TimeTracking;
using FS.TimeTracking.Abstractions.Interfaces.Application.Services.Chart;
using FS.TimeTracking.Api.REST.Routing;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Api.REST.Controllers.Chart;

/// <inheritdoc cref="IIssueChartService" />
/// <seealso cref="ControllerBase" />
/// <seealso cref="IIssueChartService" />
[V1ApiController]
public class IssueChartController : ControllerBase, IIssueChartService
{
    private readonly IIssueChartService _chartService;

    /// <summary>
    /// Initializes a new instance of the <see cref="OrderChartController"/> class.
    /// </summary>
    /// <param name="chartService">The chart service.</param>
    public IssueChartController(IIssueChartService chartService)
        => _chartService = chartService;

    /// <inheritdoc />
    [HttpGet]
    public async Task<List<IssueWorkTimeDto>> GetWorkTimesPerIssue([FromQuery] EntityFilter<TimeSheetDto> timeSheetFilter, [FromQuery] EntityFilter<ProjectDto> projectFilter, [FromQuery] EntityFilter<CustomerDto> customerFilter, [FromQuery] EntityFilter<ActivityDto> activityFilter, [FromQuery] EntityFilter<OrderDto> orderFilter, [FromQuery] EntityFilter<HolidayDto> holidayFilter, CancellationToken cancellationToken = default)
        => await _chartService.GetWorkTimesPerIssue(timeSheetFilter, projectFilter, customerFilter, activityFilter, orderFilter, holidayFilter, cancellationToken);
}