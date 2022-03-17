#if DEBUG
using FS.TimeTracking.Abstractions.Interfaces.Application.Services.Shared;
using FS.TimeTracking.Api.REST.Routing;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Api.REST.Controllers.Shared
{
    /// <inheritdoc cref="IDebugService" />
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    /// <seealso cref="IDebugService" />
    [V1ApiController]
    public class DebugController : ControllerBase, IDebugService
    {
        private readonly IDebugService _debugService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DebugController"/> class.
        /// </summary>
        /// <param name="debugService">The dev test service.</param>
        public DebugController(IDebugService debugService)
            => _debugService = debugService;

        /// <inheritdoc />
        [HttpGet]
        public async Task<object> TestMethod(CancellationToken cancellationToken = default)
            => await _debugService.TestMethod(cancellationToken);
    }
}
#endif