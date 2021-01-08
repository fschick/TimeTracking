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

        /// <summary>
        /// Method to simulate a long running operation.
        /// </summary>
        /// <param name="milliseconds">Time in milliseconds to wait before the method returns.</param>
        /// <param name="result">The result.</param>
        /// <param name="cancellationToken"> A <see cref="CancellationToken" /> to observe while waiting for the task to complete. </param>
        /// <returns>The result given by <paramref name="result"/>.</returns>
        Task<string> LongRunningOperation(int milliseconds, string result, CancellationToken cancellationToken = default);
    }
}
#endif