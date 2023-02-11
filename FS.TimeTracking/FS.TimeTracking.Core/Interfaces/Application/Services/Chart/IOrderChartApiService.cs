using FS.TimeTracking.Abstractions.DTOs.Chart;
using FS.TimeTracking.Core.Models.Filter;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Core.Interfaces.Application.Services.Chart;

/// <summary>
/// Order specific chart service
/// </summary>
public interface IOrderChartApiService
{
    /// <summary>
    /// Gets the work times grouped by order.
    /// </summary>
    /// <param name="filters">Filters applied to result.</param>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    Task<List<OrderWorkTimeDto>> GetWorkTimesPerOrder(TimeSheetFilterSet filters, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets personal workdays count.
    /// </summary>
    /// <param name="filters">Filters applied to result.</param>
    /// <param name="startDate">The start date.</param>
    /// <param name="endDate">The end date.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    Task<int> GetPersonalWorkdaysCount(TimeSheetFilterSet filters, DateTimeOffset startDate, DateTimeOffset endDate, CancellationToken cancellationToken);
}