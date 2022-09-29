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

/// <inheritdoc cref="IIssueChartService" />
/// <seealso cref="ControllerBase" />
/// <seealso cref="IIssueChartService" />
[V1ApiController]
[ExcludeFromCodeCoverage]
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
    public async Task<List<IssueWorkTimeDto>> GetWorkTimesPerIssue([FromQuery] TimeSheetFilterSet filters, CancellationToken cancellationToken = default)
        => await _chartService.GetWorkTimesPerIssue(filters, cancellationToken);
}