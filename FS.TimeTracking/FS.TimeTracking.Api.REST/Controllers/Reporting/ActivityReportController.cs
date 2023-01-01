using FS.TimeTracking.Abstractions.DTOs.Reporting;
using FS.TimeTracking.Api.REST.Routing;
using FS.TimeTracking.Core.Interfaces.Application.Services.Reporting;
using FS.TimeTracking.Core.Models.Filter;
using FS.TimeTracking.Report.Client.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Api.REST.Controllers.Reporting;

/// <summary>
/// A controller for handling time sheet reports.
/// </summary>
[ApiV1Controller]
[FeatureGate(Features.Reporting)]
[ExcludeFromCodeCoverage]
public class ActivityReportController : ControllerBase, IActivityReportService
{
    private readonly IActivityReportService _activityReportService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ActivityReportController"/> class.
    /// </summary>
    /// <param name="activityReportService">The activity report service.</param>
    public ActivityReportController(IActivityReportService activityReportService)
        => _activityReportService = activityReportService;

    /// <inheritdoc />
    [HttpGet]
    public async Task<List<ActivityReportGridDto>> GetCustomersHavingTimeSheets([FromQuery] TimeSheetFilterSet filters, string language, CancellationToken cancellationToken = default)
        => await _activityReportService.GetCustomersHavingTimeSheets(filters, language, cancellationToken);

    /// <inheritdoc />
    [HttpGet]
    public async Task<FileResult> GetActivityReport([FromQuery] TimeSheetFilterSet filters, string language, [Required] ActivityReportType reportType = ActivityReportType.Detailed, CancellationToken cancellationToken = default)
        => await _activityReportService.GetActivityReport(filters, language, reportType, cancellationToken);

    /// <inheritdoc />
    [HttpGet]
    public async Task<ReportPreviewDto> GetActivityReportPreview([FromQuery] TimeSheetFilterSet filters, string language, [Required] ActivityReportType reportType = ActivityReportType.Detailed, CancellationToken cancellationToken = default)
        => await _activityReportService.GetActivityReportPreview(filters, language, reportType, cancellationToken);

    /// <inheritdoc />
    [HttpGet]
    public async Task<ActivityReportDto> GetActivityReportData([FromQuery] TimeSheetFilterSet filters, string language, [Required] ActivityReportType reportType = ActivityReportType.Detailed, CancellationToken cancellationToken = default)
        => await _activityReportService.GetActivityReportData(filters, language, reportType, cancellationToken);
}