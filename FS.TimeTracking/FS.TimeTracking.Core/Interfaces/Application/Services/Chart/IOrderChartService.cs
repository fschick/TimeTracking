﻿using FS.TimeTracking.Abstractions.DTOs.Chart;
using FS.TimeTracking.Core.Models.Application.Chart;
using FS.TimeTracking.Core.Models.Filter;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Core.Interfaces.Application.Services.Chart;

/// <summary>
/// Order specific chart service
/// </summary>
public interface IOrderChartService
{
    /// <summary>
    /// Gets the work times grouped by order.
    /// </summary>
    /// <param name="filters">Filters applied to result.</param>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    Task<List<OrderWorkTimeDto>> GetWorkTimesPerOrder(TimeSheetFilterSet filters, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the planned times per order.
    /// </summary>
    /// <param name="filter">The filter to use.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    Task<List<OrderWorkTime>> GetPlannedTimesPerOrder(ChartFilter filter, CancellationToken cancellationToken);

    /// <summary>
    /// Gets the worked times per order.
    /// </summary>
    /// <param name="filter">The filter to use.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    Task<List<OrderWorkTime>> GetWorkedTimesPerOrder(ChartFilter filter, CancellationToken cancellationToken);

    /// <summary>
    /// Gets personal workdays count.
    /// </summary>
    /// <param name="startDate">The start date.</param>
    /// <param name="endDate">The end date.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    Task<int> GetPersonalWorkdaysCount(DateTimeOffset startDate, DateTimeOffset endDate, CancellationToken cancellationToken);
}