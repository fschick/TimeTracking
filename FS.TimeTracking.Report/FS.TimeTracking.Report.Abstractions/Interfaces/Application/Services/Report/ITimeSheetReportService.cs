using FS.TimeTracking.Shared.DTOs.Report;
using Stimulsoft.Report;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Report.Abstractions.Interfaces.Application.Services.Report;

/// <summary>
/// Time sheet report service.
/// </summary>
public interface ITimeSheetReportService
{
    /// <summary>
    /// Generates a full time sheet report.
    /// </summary>
    /// <param name="source">Source for the report.</param>
    /// <param name="cancellationToken"> a token that allows processing to be cancelled.</param>
    Task<StiReport> GetTimeSheetReport(TimeSheetReportDto source, CancellationToken cancellationToken = default);
}