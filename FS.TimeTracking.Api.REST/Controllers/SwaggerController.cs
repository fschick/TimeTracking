using FS.TimeTracking.Shared.Routing;
using Microsoft.AspNetCore.Mvc;

namespace FS.TimeTracking.Api.REST.Controllers
{
    /// <summary>
    /// No services.
    /// </summary>
    /// <seealso cref="ControllerBase" />
    [ApiController]
    [ApiExplorerSettings(GroupName = Routes.API_VERSION)]
    public class SwaggerController : ControllerBase
    {
        /// <summary>
        /// Redirects to OpenAPI UI.
        /// </summary>
        /// <returns></returns>
        [HttpGet(Routes.Swagger.SwaggerUi), ApiExplorerSettings(IgnoreApi = true)]
        public RedirectResult RedirectToOpenApiUi()
            => RedirectPermanent($"../{Routes.OpenApi.OpenApiUi}");
    }
}
