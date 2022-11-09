﻿#if DEBUG
using FS.TimeTracking.Core.Extensions;
using FS.TimeTracking.Core.Interfaces.Application.Services.Shared;
using FS.TimeTracking.Core.Interfaces.Repository.Services.Database;
using FS.TimeTracking.Core.Models.Application.MasterData;
using FS.TimeTracking.Core.Models.Application.TimeTracking;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Application.Services.Shared
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public class DebugService : IDebugService
    {
        private readonly IDbRepository _dbRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="DebugService"/> class.
        /// </summary>
        /// <param name="dbRepository">The repository.</param>
        /// <autogeneratedoc />
        public DebugService(IDbRepository dbRepository)
            => _dbRepository = dbRepository;

        /// <inheritdoc />
        public async Task<object> TestMethod(CancellationToken cancellationToken = default)
        {
            var d1 = await _dbRepository.Get((Order x) => x.StartDateLocal, cancellationToken: cancellationToken);
            var d2 = await _dbRepository.Get((Order x) => x.StartDateLocal.ToUtc(120), cancellationToken: cancellationToken);
            var d3 = await _dbRepository.Get((Order x) => x.StartDateLocal.ToUtc(x.StartDateOffset), cancellationToken: cancellationToken);
            var d4 = await _dbRepository.Get((TimeSheet x) => x.EndDateLocal.ToUtc(x.EndDateOffset.Value), cancellationToken: cancellationToken);
            return Task.FromResult<object>(1);
        }
    }
}
#endif