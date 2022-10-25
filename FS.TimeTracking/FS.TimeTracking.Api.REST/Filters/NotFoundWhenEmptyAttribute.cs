using FS.TimeTracking.Api.REST.Models;
using FS.TimeTracking.Core.Models.Application.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace FS.TimeTracking.Api.REST.Filters;

[AttributeUsage(AttributeTargets.Method)]
internal class NotFoundWhenEmptyAttribute : ResultFilterAttribute
{
    public override void OnResultExecuting(ResultExecutingContext context)
    {
        base.OnResultExecuting(context);
        if (context.Result is ObjectResult { Value: null })
            context.Result = new NotFoundObjectResult(new ApplicationError { Code = ApplicationErrorCode.NotFound });
    }
}