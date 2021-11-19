using FS.TimeTracking.Api.REST.Routing;
using FS.TimeTracking.Api.REST.Startup;
using Microsoft.AspNetCore.Mvc;

namespace FS.TimeTracking.Api.REST.Controllers.Shared;

/// <summary>
/// No services.
/// </summary>
/// <seealso cref="ControllerBase" />
[V1ApiController]
public class SwaggerController : ControllerBase
{
    /// <summary>
    /// Redirects to OpenAPI UI.
    /// </summary>
    /// <returns></returns>
    [HttpGet(OpenApi.SWAGGER_UI_ROUTE), ApiExplorerSettings(IgnoreApi = true)]
    public RedirectResult RedirectToOpenApiUi()
        => RedirectPermanent($"../{OpenApi.OPEN_API_UI_ROUTE}");
}