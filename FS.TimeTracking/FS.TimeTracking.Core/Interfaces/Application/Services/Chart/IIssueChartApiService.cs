using FS.TimeTracking.Abstractions.DTOs.Chart;
using FS.TimeTracking.Core.Models.Filter;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Core.Interfaces.Application.Services.Chart;

/// <summary>
/// Issue specific chart service
/// </summary>
public interface IIssueChartApiService
{
    /// <summary>
    /// Gets the work times grouped by customer.
    /// </summary>
    /// <param name="filters">Filters applied to result.</param>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    Task<List<IssueWorkTimeDto>> GetWorkTimesPerIssue(TimeSheetFilterSet filters, CancellationToken cancellationToken = default);
}