using FS.TimeTracking.Shared.Extensions;
using FS.TimeTracking.Shared.Interfaces.Application.Services;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Application.Services
{
    /// <inheritdoc />
    public class InformationService : IInformationService
    {
        /// <inheritdoc />
        public Task<string> GetProductName(CancellationToken cancellationToken = default)
            => Task.FromResult(AssemblyExtensions.GetProgramProduct());

        /// <inheritdoc />
        public Task<string> GetProductVersion(CancellationToken cancellationToken = default)
            => Task.FromResult(AssemblyExtensions.GetProgramProductVersion());

        /// <inheritdoc />
        public Task<string> GetProductCopyright(CancellationToken cancellationToken = default)
            => Task.FromResult(AssemblyExtensions.GetProgramCopyright());
    }
}
