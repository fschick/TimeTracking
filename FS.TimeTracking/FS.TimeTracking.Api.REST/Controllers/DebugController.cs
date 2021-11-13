﻿#if DEBUG
using FS.TimeTracking.Api.REST.Routing;
using FS.TimeTracking.Shared.Interfaces.Application.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using FS.TimeTracking.Shared.Interfaces.Application.Services.Shared;

namespace FS.TimeTracking.Api.REST.Controllers
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
        public Task<object> TestMethod(CancellationToken cancellationToken = default)
            => _debugService.TestMethod(cancellationToken);
    }
}
#endif