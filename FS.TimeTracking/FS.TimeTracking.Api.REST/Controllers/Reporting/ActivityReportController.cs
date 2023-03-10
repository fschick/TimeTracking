using FS.Authentication.OneTimeToken.Abstractions.Models;
using FS.TimeTracking.Abstractions.Constants;
using FS.TimeTracking.Abstractions.DTOs.Reporting;
using FS.TimeTracking.Api.REST.Routing;
using FS.TimeTracking.Core.Interfaces.Application.Services.Reporting;
using FS.TimeTracking.Core.Models.Filter;
using FS.TimeTracking.Report.Client.Model;
using Microsoft.AspNetCore.Authorization;
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
[Authorize]
[FeatureGate(Features.Reporting)]
[ExcludeFromCodeCoverage]
public class ActivityReportController : ControllerBase, IActivityReportApiService
{
    private readonly IActivityReportApiService _activityReportService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ActivityReportController"/> class.
    /// </summary>
    /// <param name="activityReportService">The activity report service.</param>
    public ActivityReportController(IActivityReportApiService activityReportService)
        => _activityReportService = activityReportService;

    /// <inheritdoc />
    [HttpGet]
    [Authorize(Roles = RoleName.REPORT_ACTIVITY_SUMMARY_VIEW + ", " + RoleName.REPORT_ACTIVITY_DETAIL_VIEW)]
    public async Task<List<ActivityReportGridDto>> GetCustomersHavingTimeSheets([FromQuery] TimeSheetFilterSet filters, string language, CancellationToken cancellationToken = default)
        => await _activityReportService.GetCustomersHavingTimeSheets(filters, language, cancellationToken);

    /// <inheritdoc />
    [HttpGet]
    [Authorize(Roles = RoleName.REPORT_ACTIVITY_SUMMARY_VIEW)]
    public async Task<string> GetDailyActivityReportDownloadToken(CancellationToken cancellationToken = default)
        => await _activityReportService.GetDailyActivityReportDownloadToken(cancellationToken);

    /// <inheritdoc />
    [HttpGet]
    [Authorize(Roles = RoleName.REPORT_ACTIVITY_DETAIL_VIEW)]
    public async Task<string> GetDetailedActivityReportDownloadToken(CancellationToken cancellationToken = default)
        => await _activityReportService.GetDetailedActivityReportDownloadToken(cancellationToken);

    /// <inheritdoc />
    [HttpGet]
    [Authorize(AuthenticationSchemes = OneTimeTokenDefaults.AuthenticationScheme, Roles = RoleName.REPORT_ACTIVITY_SUMMARY_VIEW)]
    public async Task<FileResult> GetDailyActivityReport([FromQuery] TimeSheetFilterSet filters, string language, CancellationToken cancellationToken = default)
        => await _activityReportService.GetDailyActivityReport(filters, language, cancellationToken);

    /// <inheritdoc />
    [HttpGet]
    [Authorize(AuthenticationSchemes = OneTimeTokenDefaults.AuthenticationScheme, Roles = RoleName.REPORT_ACTIVITY_DETAIL_VIEW)]
    public async Task<FileResult> GetDetailedActivityReport([FromQuery] TimeSheetFilterSet filters, string language, CancellationToken cancellationToken = default)
        => await _activityReportService.GetDetailedActivityReport(filters, language, cancellationToken);

    /// <inheritdoc />
    [HttpGet]
    [Authorize(Roles = RoleName.REPORT_ACTIVITY_SUMMARY_VIEW)]
    public async Task<ReportPreviewDto> GetDailyActivityReportPreview([FromQuery] TimeSheetFilterSet filters, string language, CancellationToken cancellationToken = default)
        => await _activityReportService.GetDailyActivityReportPreview(filters, language, cancellationToken);

    /// <inheritdoc />
    [HttpGet]
    [Authorize(Roles = RoleName.REPORT_ACTIVITY_DETAIL_VIEW)]
    public async Task<ReportPreviewDto> GetDetailedActivityReportPreview([FromQuery] TimeSheetFilterSet filters, string language, CancellationToken cancellationToken = default)
        => await _activityReportService.GetDetailedActivityReportPreview(filters, language, cancellationToken);

    /// <inheritdoc />
    [HttpGet]
    [Authorize(Roles = RoleName.REPORT_ACTIVITY_RAW_DATA_VIEW)]
    public async Task<ActivityReportDto> GetActivityReportData([FromQuery] TimeSheetFilterSet filters, string language, [Required] ActivityReportType reportType = ActivityReportType.Detailed, CancellationToken cancellationToken = default)
        => await _activityReportService.GetActivityReportData(filters, language, reportType, cancellationToken);
}