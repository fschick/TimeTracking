using FS.TimeTracking.Report.Abstractions.Interfaces.Application.Services.Report;
using FS.TimeTracking.Report.Api.REST.Routing;
using FS.TimeTracking.Shared.DTOs.Report;
using Microsoft.AspNetCore.Mvc;
using Stimulsoft.Report;
using Stimulsoft.Report.Export;
using Stimulsoft.Report.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Report.Api.REST.Controllers.Report;

/// <summary>
/// A controller for handling time sheet reports.
/// </summary>
[V1ApiController]
public class ActivityReportController : ControllerBase, IActivityReportService
{
    private readonly IActivityReportService _activityReportService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ActivityReportController"/> class.
    /// </summary>
    /// <param name="activityReportService">The time sheet report service.</param>
    public ActivityReportController(IActivityReportService activityReportService)
        => _activityReportService = activityReportService;

    /// <inheritdoc cref="IActivityReportService.GetActivityReport" />
    [HttpPost]
    public async Task<IActionResult> GenerateActivityReport(ActivityReportDto source, CancellationToken cancellationToken = default)
    {
        using var report = await _activityReportService.GetActivityReport(source, cancellationToken);
        return StiNetCoreReportResponse.ResponseAsPdf(report);
    }

    /// <inheritdoc cref="IActivityReportService.GetActivityReport" />
    [HttpPost]
    public async Task<IActionResult> GenerateActivityReportPreview(ActivityReportDto source, CancellationToken cancellationToken = default)
    {
        using var report = await _activityReportService.GetActivityReport(source, cancellationToken);
        return StiNetCoreReportResponse.ResponseAsHtml5(report, new StiHtmlExportSettings() { PageRange = new StiPagesRange("1-4") });
        //return StiNetCoreReportResponse.ResponseAsSvg(report, new StiImageExportSettings { ImageFormat = StiImageFormat.Color, ImageType = StiImageType.Svg, PageRange = new StiPagesRange("1-4") });
        //await report.RenderAsync(1, 4);
        //report.ExportDocument(StiExportFormat.ImagePng, "", new StiPngExportSettings { });

        //var result = StiNetCoreReportResponse.ResponseAsPng(report, new StiImageExportSettings(StiImageType.Png) { MultipleFiles = false, PageRange = new StiPagesRange("1-2") }, false);
        //return result;
    }

    [NonAction]
    Task<StiReport> IActivityReportService.GetActivityReport(ActivityReportDto source, CancellationToken cancellationToken)
        => throw new System.NotImplementedException($"Use {nameof(GenerateActivityReport)} instead");
}