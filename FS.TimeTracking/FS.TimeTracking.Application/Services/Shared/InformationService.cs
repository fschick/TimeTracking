using System.Threading;
using System.Threading.Tasks;
using FS.TimeTracking.Shared.Extensions;
using FS.TimeTracking.Shared.Interfaces.Application.Services;
using FS.TimeTracking.Shared.Interfaces.Application.Services.Shared;

namespace FS.TimeTracking.Application.Services.Shared
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
