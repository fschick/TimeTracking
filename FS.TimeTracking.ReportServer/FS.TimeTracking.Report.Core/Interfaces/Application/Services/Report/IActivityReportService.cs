using FS.TimeTracking.Report.Abstractions.DTOs.Reports;
using Stimulsoft.Report;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Report.Core.Interfaces.Application.Services.Report;

/// <summary>
/// Time sheet report service.
/// </summary>
public interface IActivityReportService
{
    /// <summary>
    /// Generates a report.
    /// </summary>
    /// <param name="source">Source for the report.</param>
    /// <param name="cancellationToken"> a token that allows processing to be cancelled.</param>
    Task<StiReport> GetActivityReport(ActivityReportDto source, CancellationToken cancellationToken = default);
}