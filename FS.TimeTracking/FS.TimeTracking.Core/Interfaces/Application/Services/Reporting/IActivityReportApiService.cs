using FS.TimeTracking.Abstractions.DTOs.Reporting;
using FS.TimeTracking.Core.Models.Filter;
using FS.TimeTracking.Report.Client.Model;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Core.Interfaces.Application.Services.Reporting;

/// <summary>
/// Time sheet report service
/// </summary>
public interface IActivityReportApiService
{
    /// <summary>
    /// Get all customers having time sheets matching the filters
    /// </summary>
    /// <param name="filters">Filters applied to result.</param>
    /// <param name="language">The language to get translations for.</param>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    Task<List<ActivityReportGridDto>> GetCustomersHavingTimeSheets(TimeSheetFilterSet filters, string language, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates an one-time access token to download a daily activity report.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    Task<string> GetDailyActivityReportDownloadToken(CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates an one-time access token to download a daily activity report.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    Task<string> GetDetailedActivityReportDownloadToken(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a daily activity report.
    /// </summary>
    /// <param name="filters">Filters applied to result.</param>
    /// <param name="language">The language to get translations for.</param>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    Task<FileResult> GetDailyActivityReport(TimeSheetFilterSet filters, string language, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a detailed activity report.
    /// </summary>
    /// <param name="filters">Filters applied to result.</param>
    /// <param name="language">The language to get translations for.</param>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    Task<FileResult> GetDetailedActivityReport(TimeSheetFilterSet filters, string language, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the preview of a daily activity report.
    /// </summary>
    /// <param name="filters">Filters applied to result.</param>
    /// <param name="language">The language to get translations for.</param>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    Task<ReportPreviewDto> GetDailyActivityReportPreview(TimeSheetFilterSet filters, string language, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the preview of a detailed activity report.
    /// </summary>
    /// <param name="filters">Filters applied to result.</param>
    /// <param name="language">The language to get translations for.</param>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    Task<ReportPreviewDto> GetDetailedActivityReportPreview(TimeSheetFilterSet filters, string language, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets data to create activity based reports.
    /// </summary>
    /// <param name="filters">Filters applied to result.</param>
    /// <param name="language">The language to get translations for.</param>
    /// <param name="reportType">The type of the activity report.</param>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    Task<ActivityReportDto> GetActivityReportData(TimeSheetFilterSet filters, string language, ActivityReportType reportType = ActivityReportType.Detailed, CancellationToken cancellationToken = default);
}