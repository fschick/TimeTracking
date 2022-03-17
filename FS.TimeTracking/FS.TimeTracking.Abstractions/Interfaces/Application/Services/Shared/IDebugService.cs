using System.Threading;
using System.Threading.Tasks;

#if DEBUG
namespace FS.TimeTracking.Abstractions.Interfaces.Application.Services.Shared
{
    /// <summary>
    /// Debug service
    /// </summary>
    public interface IDebugService
    {
        /// <summary>
        /// Debug tests method.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        Task<object> TestMethod(CancellationToken cancellationToken = default);
    }
}
#endif