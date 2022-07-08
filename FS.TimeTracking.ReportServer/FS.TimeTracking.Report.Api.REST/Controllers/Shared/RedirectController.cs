using FS.TimeTracking.Report.Api.REST.Routing;
using FS.TimeTracking.Report.Api.REST.Startup;
using Microsoft.AspNetCore.Mvc;

namespace FS.TimeTracking.Report.Api.REST.Controllers.Shared;

/// <summary>
/// Redirection rules.
/// </summary>
/// <seealso cref="ControllerBase" />
[V1ApiController]
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
        var redirectUrl = $"/{OpenApi.OPEN_API_UI_ROUTE}{path}{query}";
        return RedirectPermanent(redirectUrl);
    }

    /// <summary>
    /// Redirects /swagger to OpenAPI UI.
    /// </summary>
    [HttpGet("/")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public RedirectResult RedirectRootToReportOpenApiUi()
    {
        var query = HttpContext.Request.QueryString;
        var redirectUrl = $"/{OpenApi.OPEN_API_UI_ROUTE}{query}";
        return RedirectPermanent(redirectUrl);
    }

    /// <summary>
    /// Redirects /swagger to OpenAPI UI.
    /// </summary>
    [HttpGet("/openapi/")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public RedirectResult RedirectOpenApiToReportOpenApiUi()
    {
        var query = HttpContext.Request.QueryString;
        var redirectUrl = $"/{OpenApi.OPEN_API_UI_ROUTE}{query}";
        return RedirectPermanent(redirectUrl);
    }
}