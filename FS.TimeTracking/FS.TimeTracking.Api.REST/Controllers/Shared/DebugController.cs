#if DEBUG
using FS.TimeTracking.Api.REST.Routing;
using FS.TimeTracking.Core.Interfaces.Application.Services.Shared;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Api.REST.Controllers.Shared
{
    /// <inheritdoc cref="IDebugApiService" />
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    /// <seealso cref="IDebugApiService" />
    [ApiV1Controller]
    [ExcludeFromCodeCoverage]
    public class DebugController : ControllerBase, IDebugApiService
    {
        private readonly IDebugApiService _debugService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DebugController"/> class.
        /// </summary>
        /// <param name="debugService">The dev test service.</param>
        public DebugController(IDebugApiService debugService)
            => _debugService = debugService;

        /// <inheritdoc />
        [HttpGet]
        public async Task<object> TestMethod(CancellationToken cancellationToken = default)
            => await _debugService.TestMethod(cancellationToken);
    }
}
#endif