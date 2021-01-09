#if DEBUG
using FS.TimeTracking.Shared.Interfaces.Application;
using FS.TimeTracking.Shared.Routing;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Api.REST.Controllers
{
    /// <inheritdoc cref="IDevTestService" />
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    /// <seealso cref="IDevTestService" />
    [ApiController]
    [ApiExplorerSettings(GroupName = Routes.API_VERSION)]
    public class DevTestController : ControllerBase, IDevTestService
    {
        private readonly IDevTestService _devTestService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DevTestController"/> class.
        /// </summary>
        /// <param name="devTestService">The dev test service.</param>
        public DevTestController(IDevTestService devTestService)
            => _devTestService = devTestService;

        /// <inheritdoc />
        [HttpGet(Routes.DevTest.TestMethod)]
        public Task<object> TestMethod(CancellationToken cancellationToken = default)
            => _devTestService.TestMethod(cancellationToken);
    }
}
#endif