using FS.TimeTracking.Api.REST.Routing;
using FS.TimeTracking.Shared.Interfaces.Application.Services.Report;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FS.TimeTracking.Api.REST.Controllers.Report;

/// <inheritdoc cref="IReportService" />
/// <seealso cref="ControllerBase" />
/// <seealso cref="IReportService" />
[V1ApiController]
public class ReportController : ControllerBase, IReportService
{
    private readonly IReportService _reportService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReportController"/> class.
    /// </summary>
    /// <param name="reportService">The report service.</param>
    public ReportController(IReportService reportService)
        => _reportService = reportService;

    /// <inheritdoc />
    [HttpGet]
    public Task GetWorkTimesPerCustomer()
        => _reportService.GetWorkTimesPerCustomer();
}