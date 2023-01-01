using System;
using System.Diagnostics.CodeAnalysis;

namespace FS.TimeTracking.Api.REST.Routing;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
[ExcludeFromCodeCoverage]
internal class ApiV1ControllerAttribute : ApiControllerAttribute
{
    public const string API_VERSION = "v1";

    protected override string ApiVersion => API_VERSION;
}