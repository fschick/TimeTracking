using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using System.Threading.Tasks;

namespace FS.TimeTracking.Api.REST.Filters;

internal class VoidToHttpNoContentFilter : IResultFilter
{
    /// <inheritdoc/>
    public void OnResultExecuting(ResultExecutingContext context)
    {
        if (context.ActionDescriptor is not ControllerActionDescriptor actionDescriptor)
            return;

        var returnType = actionDescriptor.MethodInfo.ReturnType;
        if (returnType == typeof(void) || returnType == typeof(Task))
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.NoContent;
    }

    /// <inheritdoc/>
    public void OnResultExecuted(ResultExecutedContext context)
    {
    }
}