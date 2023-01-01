using FS.TimeTracking.Abstractions.DTOs.Chart;
using FS.TimeTracking.Api.REST.Routing;
using FS.TimeTracking.Core.Interfaces.Application.Services.Chart;
using FS.TimeTracking.Core.Models.Filter;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Api.REST.Controllers.Chart;

/// <inheritdoc cref="ICustomerChartService" />
/// <seealso cref="ControllerBase" />
/// <seealso cref="ICustomerChartService" />
[ApiV1Controller]
[ExcludeFromCodeCoverage]
public class CustomerChartController : ControllerBase, ICustomerChartService
{
    private readonly ICustomerChartService _chartService;

    /// <summary>
    /// Initializes a new instance of the <see cref="OrderChartController"/> class.
    /// </summary>
    /// <param name="chartService">The chart service.</param>
    public CustomerChartController(ICustomerChartService chartService)
        => _chartService = chartService;

    /// <inheritdoc />
    [HttpGet]
    public async Task<List<CustomerWorkTimeDto>> GetWorkTimesPerCustomer([FromQuery] TimeSheetFilterSet filters, CancellationToken cancellationToken = default)
        => await _chartService.GetWorkTimesPerCustomer(filters, cancellationToken);
}