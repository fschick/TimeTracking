using FS.TimeTracking.Api.REST.Routing;
using FS.TimeTracking.Shared.Interfaces.Application.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Api.REST.Controllers
{
    /// <inheritdoc cref="IInformationService" />
    /// <seealso cref="ControllerBase" />
    /// <seealso cref="IInformationService" />
    [V1ApiController]
    public class InformationController : ControllerBase, IInformationService
    {
        private readonly IInformationService _informationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="InformationController"/> class.
        /// </summary>
        /// <param name="informationService">The information service.</param>
        public InformationController(IInformationService informationService)
            => _informationService = informationService;

        /// <inheritdoc />
        [HttpGet]
        public Task<string> GetProductName(CancellationToken cancellationToken = default)
            => _informationService.GetProductName(cancellationToken);

        /// <inheritdoc />
        [HttpGet]
        public Task<string> GetProductVersion(CancellationToken cancellationToken = default)
            => _informationService.GetProductVersion(cancellationToken);

        /// <inheritdoc />
        [HttpGet]
        public Task<string> GetProductCopyright(CancellationToken cancellationToken = default)
            => _informationService.GetProductCopyright(cancellationToken);
    }
}
