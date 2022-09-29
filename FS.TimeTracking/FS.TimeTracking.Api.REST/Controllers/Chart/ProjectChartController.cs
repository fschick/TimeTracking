using FS.TimeTracking.Abstractions.DTOs.Chart;
using FS.TimeTracking.Api.REST.Routing;
using FS.TimeTracking.Core.Interfaces.Application.Services.Chart;
using FS.TimeTracking.Core.Models.Filter;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Api.REST.Controllers.Chart;

/// <inheritdoc cref="IProjectChartService" />
/// <seealso cref="ControllerBase" />
/// <seealso cref="IProjectChartService" />
[V1ApiController]
[ExcludeFromCodeCoverage]
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
    public async Task<List<ProjectWorkTimeDto>> GetWorkTimesPerProject([FromQuery] TimeSheetFilterSet filters, CancellationToken cancellationToken = default)
        => await _chartService.GetWorkTimesPerProject(filters, cancellationToken);
}