using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using System.Threading.Tasks;

namespace FS.TimeTracking.Api.REST.Filters
{
    internal class AddRequestIdToHeaderFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            context.HttpContext.Response.OnStarting(() =>
            {
                var accessControlHeader = context.HttpContext.Response.Headers["Access-Control-Expose-Headers"];
                accessControlHeader = StringValues.Concat(accessControlHeader, "Request-Id");
                context.HttpContext.Response.Headers["Access-Control-Expose-Headers"] = accessControlHeader;
                context.HttpContext.Response.Headers.Add("Request-Id", context.HttpContext.TraceIdentifier);
                return Task.CompletedTask;
            });
        }
    }
}