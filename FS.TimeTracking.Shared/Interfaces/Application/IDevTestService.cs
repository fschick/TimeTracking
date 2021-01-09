using System.Threading;
using System.Threading.Tasks;

#if DEBUG
namespace FS.TimeTracking.Shared.Interfaces.Application
{
    /// <summary>
    /// Test service for development.
    /// </summary>
    public interface IDevTestService
    {
        /// <summary>
        /// Empty test method.
        /// </summary>
        /// <param name="cancellationToken"> A <see cref="CancellationToken" /> to observe while waiting for the task to complete. </param>
        /// <returns></returns>
        Task<object> TestMethod(CancellationToken cancellationToken = default);
    }
}
#endif