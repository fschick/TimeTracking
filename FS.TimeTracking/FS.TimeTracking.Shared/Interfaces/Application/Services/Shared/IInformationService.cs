using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Shared.Interfaces.Application.Services.Shared
{
    /// <summary>
    /// Information services.
    /// </summary>
    public interface IInformationService

    {
        /// <summary>
        /// Gets the name of the product.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        Task<string> GetProductName(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the product version.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        Task<string> GetProductVersion(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the copyright for the product.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        Task<string> GetProductCopyright(CancellationToken cancellationToken = default);
    }
}
