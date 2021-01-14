using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace FS.TimeTracking.Api.REST.Startup
{
    internal class ActionExtensionOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var action = context.MethodInfo.Name;
            var lowercasedAction = char.ToLower(action[0]) + action.Substring(1);
            operation.Extensions.Add("x-csharp-action", new OpenApiString(lowercasedAction));
        }
    }
}
