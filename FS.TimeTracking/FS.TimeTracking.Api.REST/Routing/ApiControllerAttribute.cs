using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using System;
using System.Diagnostics.CodeAnalysis;

namespace FS.TimeTracking.Api.REST.Routing;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
[ExcludeFromCodeCoverage]
internal abstract class ApiControllerAttribute : ControllerAttribute, IApiBehaviorMetadata, IRouteTemplateProvider, IApiDescriptionGroupNameProvider
{
    public const string API_PREFIX = "api";

    protected abstract string ApiVersion { get; }

    /// <inheritdoc />
    public string GroupName => ApiVersion;

    /// <inheritdoc />
    public string Template => $"{API_PREFIX}/{ApiVersion}/[controller]/[action]";

    /// <inheritdoc />
    public int? Order => 0;

    /// <inheritdoc />
    public string Name => "[controller]_[action]";
}