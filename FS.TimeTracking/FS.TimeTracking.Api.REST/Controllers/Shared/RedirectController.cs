using FS.TimeTracking.Api.REST.Routing;
using FS.TimeTracking.Api.REST.Startup;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace FS.TimeTracking.Api.REST.Controllers.Shared;

/// <summary>
/// Redirection rules.
/// </summary>
/// <seealso cref="ControllerBase" />
[V1ApiController]
[ExcludeFromCodeCoverage]
public class RedirectController : ControllerBase
{
    /// <summary>
    /// Redirects /swagger to OpenAPI UI.
    /// </summary>
    [HttpGet($"/{OpenApi.SWAGGER_UI_ROUTE}{{**path}}")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public RedirectResult RedirectSwaggerToOpenApiUi(string path)
    {
        var query = HttpContext.Request.QueryString;
        var redirectUrl = $"/{OpenApi.OPEN_API_UI_ROUTE}/{path}{query}";
        return RedirectPermanent(redirectUrl);
    }
}