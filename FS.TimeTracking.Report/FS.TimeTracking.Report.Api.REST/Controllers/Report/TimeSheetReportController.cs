using FS.TimeTracking.Report.Abstractions.Interfaces.Application.Services.Report;
using FS.TimeTracking.Report.Api.REST.Routing;
using FS.TimeTracking.Shared.DTOs.Report;
using Microsoft.AspNetCore.Mvc;
using Stimulsoft.Report;
using Stimulsoft.Report.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Report.Api.REST.Controllers.Report;

/// <summary>
/// A controller for handling time sheet reports.
/// </summary>
[V1ApiController]
public class TimeSheetReportController : ControllerBase, ITimeSheetReportService
{
    private readonly ITimeSheetReportService _timeSheetReportService;

    /// <summary>
    /// Initializes a new instance of the <see cref="TimeSheetReportController"/> class.
    /// </summary>
    /// <param name="timeSheetReportService">The time sheet report service.</param>
    public TimeSheetReportController(ITimeSheetReportService timeSheetReportService)
        => _timeSheetReportService = timeSheetReportService;


    /// <inheritdoc cref="ITimeSheetReportService.GetTimeSheetReport" />
    [HttpPost]
    public async Task<IActionResult> GenerateTimeSheetReport(TimeSheetReportDto source, CancellationToken cancellationToken = default)
    {
        using var report = await _timeSheetReportService.GetTimeSheetReport(source, cancellationToken);
        return StiNetCoreReportResponse.ResponseAsPdf(report);
    }

    [NonAction]
    Task<StiReport> ITimeSheetReportService.GetTimeSheetReport(TimeSheetReportDto source, CancellationToken cancellationToken)
        => throw new System.NotImplementedException($"Use {nameof(GenerateTimeSheetReport)} instead");
}