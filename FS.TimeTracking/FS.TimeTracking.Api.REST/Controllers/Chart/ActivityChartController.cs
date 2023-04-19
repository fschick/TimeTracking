﻿using FS.TimeTracking.Abstractions.Constants;
using FS.TimeTracking.Abstractions.DTOs.Chart;
using FS.TimeTracking.Api.REST.Routing;
using FS.TimeTracking.Core.Interfaces.Application.Services.Chart;
using FS.TimeTracking.Core.Models.Filter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Api.REST.Controllers.Chart;

/// <inheritdoc cref="IActivityChartService" />
/// <seealso cref="ControllerBase" />
/// <seealso cref="IActivityChartService" />
[ApiV1Controller]
[Authorize(Roles = RoleNames.CHARTS_BY_ACTIVITY_VIEW)]
[ExcludeFromCodeCoverage]
public class ActivityChartController : ControllerBase, IActivityChartService
{
    private readonly IActivityChartService _chartService;

    /// <summary>
    /// Initializes a new instance of the <see cref="OrderChartController"/> class.
    /// </summary>
    /// <param name="chartService">The chart service.</param>
    public ActivityChartController(IActivityChartService chartService)
        => _chartService = chartService;

    /// <inheritdoc />
    [HttpGet]
    public async Task<List<ActivityWorkTimeDto>> GetWorkTimesPerActivity([FromQuery] TimeSheetFilterSet filters, CancellationToken cancellationToken = default)
        => await _chartService.GetWorkTimesPerActivity(filters, cancellationToken);
}