using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;

namespace FS.TimeTracking.Api.REST.Filters;

internal class AddRequestIdToHeaderFilter : IResultFilter
{
    public void OnResultExecuting(ResultExecutingContext context)
    {
        var accessControlHeader = context.HttpContext.Response.Headers["Access-Control-Expose-Headers"];
        accessControlHeader = StringValues.Concat(accessControlHeader, "Request-Id");
        context.HttpContext.Response.Headers["Access-Control-Expose-Headers"] = accessControlHeader;
        context.HttpContext.Response.Headers.Append("Request-Id", context.HttpContext.TraceIdentifier);
    }

    public void OnResultExecuted(ResultExecutedContext context)
    {
    }
}