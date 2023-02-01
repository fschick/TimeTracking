using FS.TimeTracking.Core.Models.Application.Chart;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Core.Interfaces.Application.Services.Chart;

/// <summary>
/// Order specific chart service
/// </summary>
public interface IOrderChartService : IOrderChartApiService
{
    /// <summary>
    /// Gets the worked times per order.
    /// </summary>
    /// <param name="filter">The filter to use.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    Task<List<OrderWorkTime>> GetWorkedTimesPerOrder(ChartFilter filter, CancellationToken cancellationToken);

    /// <summary>
    /// Gets the planned times per order.
    /// </summary>
    /// <param name="filter">The filter to use.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    Task<List<OrderWorkTime>> GetPlannedTimesPerOrder(ChartFilter filter, CancellationToken cancellationToken);
}