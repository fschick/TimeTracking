using System.Threading.Tasks;

namespace FS.TimeTracking.Shared.Interfaces.Application.Services.Report;

/// <summary>
/// Reporting service
/// </summary>
public interface IReportService
{
    /// <summary>
    /// Gets the work times grouped by customer.
    /// </summary>
    Task GetWorkTimesPerCustomer();
}