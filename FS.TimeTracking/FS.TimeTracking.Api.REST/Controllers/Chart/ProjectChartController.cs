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

/// <inheritdoc cref="IProjectChartService" />
/// <seealso cref="ControllerBase" />
/// <seealso cref="IProjectChartService" />
[V1ApiController]
public class ProjectChartController : ControllerBase, IProjectChartService
{
    private readonly IProjectChartService _chartService;

    /// <summary>
    /// Initializes a new instance of the <see cref="OrderChartController"/> class.
    /// </summary>
    /// <param name="chartService">The chart service.</param>
    public ProjectChartController(IProjectChartService chartService)
        => _chartService = chartService;

    /// <inheritdoc />
    [HttpGet]
    public async Task<List<ProjectWorkTimeDto>> GetWorkTimesPerProject([FromQuery] EntityFilter<TimeSheetDto> timeSheetFilter, [FromQuery] EntityFilter<ProjectDto> projectFilter, [FromQuery] EntityFilter<CustomerDto> customerFilter, [FromQuery] EntityFilter<ActivityDto> activityFilter, [FromQuery] EntityFilter<OrderDto> orderFilter, [FromQuery] EntityFilter<HolidayDto> holidayFilter, CancellationToken cancellationToken = default)
        => await _chartService.GetWorkTimesPerProject(timeSheetFilter, projectFilter, customerFilter, activityFilter, orderFilter, holidayFilter, cancellationToken);
}