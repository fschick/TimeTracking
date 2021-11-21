using FS.TimeTracking.Shared.Interfaces.Application.Services.Report;
using FS.TimeTracking.Shared.Interfaces.Repository.Services;
using FS.TimeTracking.Shared.Models.TimeTracking;
using System.Linq;
using System.Threading.Tasks;

namespace FS.TimeTracking.Application.Services.Report;

/// <inheritdoc />
public class ReportService : IReportService
{
    private readonly IRepository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReportService"/> class.
    /// </summary>
    /// <param name="repository">The repository.</param>
    public ReportService(IRepository repository)
        => _repository = repository;

    /// <inheritdoc />
    public async Task GetWorkTimesPerCustomer()
    {
        var r = await _repository
            .GetGrouped(
                groupBy: (TimeSheet x) => new { x.Project.CustomerId, x.Project.Customer.Title },
                select: x => new { x.Key.CustomerId, x.Key.Title, Minutes = x.Sum(f => f.StartDateLocal.Minute) }
            );
    }
}