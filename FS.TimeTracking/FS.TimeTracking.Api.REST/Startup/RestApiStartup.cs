using FS.TimeTracking.Abstractions.Constants;
using FS.TimeTracking.Api.REST.Extensions;
using FS.TimeTracking.Api.REST.Filters;
using FS.TimeTracking.Core.Interfaces.Application.Services.Shared;
using FS.TimeTracking.Core.Models.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FS.TimeTracking.Api.REST.Startup;

internal static class RestApiStartup
{
    public static WebApplication RegisterRestApiRoutes(this WebApplication webApplication)
    {
        var controllerBuilder = webApplication.MapControllers();

        var configuration = webApplication.Services.GetRequiredService<IOptions<TimeTrackingConfiguration>>().Value;
        if (!configuration.Features.Authorization)
            controllerBuilder.AllowAnonymous();

        return webApplication;
    }

    public static IServiceCollection RegisterRestApiController(this IServiceCollection services)
    {
        services
            .AddControllers(options =>
            {
                options.OutputFormatters.RemoveType<StringOutputFormatter>();
                options.Filters.Add<AddRequestIdToHeaderFilter>();
                options.Filters.Add<VoidToHttpNoContentFilter>();
                options.Filters.Add<ExceptionToHttpResultFilter>();
            })
            .AddPlainquire()
            .AddNewtonsoftJson(options =>
            {
                var camelCase = new CamelCaseNamingStrategy();
                options.SerializerSettings.Converters.Add(new StringEnumConverter(camelCase));
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });

        return services;
    }

    public static IServiceCollection RegisterAuthorizationPolicies(this IServiceCollection services)
    {
        services.
            AddAuthorization(options =>
            {
                foreach (var permissionName in PermissionName.CrudServicePermissionNames)
                    options.ExtendPolicy(permissionName, policy => ConfigureCrudServicePolicy(policy, permissionName));
            });

        return services;
    }

    private static void ConfigureCrudServicePolicy(AuthorizationPolicyBuilder policy, string policyName)
    {
        policy.RequireAssertion(context =>
        {
            var httpContext = context.Resource as HttpContext;
            var endpoint = httpContext?.GetEndpoint();
            var actionDescriptor = endpoint?.Metadata.GetMetadata<ControllerActionDescriptor>();
            if (actionDescriptor == null || !actionDescriptor.IsCrudModelService())
                throw new InvalidOperationException("Policy can only be applied to controllers implements interface 'ICrudModelService'.");

            switch (actionDescriptor.ActionName)
            {
                case nameof(ICrudModelService<object, object, object>.Get):
                case nameof(ICrudModelService<object, object, object>.GetGridFiltered):
                case nameof(ICrudModelService<object, object, object>.GetGridItem):
                    return context.User.IsInRole($"{policyName}-{PermissionScope.VIEW}");
                case nameof(ICrudModelService<object, object, object>.Create):
                case nameof(ICrudModelService<object, object, object>.Update):
                case nameof(ICrudModelService<object, object, object>.Delete):
                    return context.User.IsInRole($"{policyName}-{PermissionScope.MANAGE}");
                default:
                    return true;
            }
        });
    }

    private static bool IsCrudModelService(this ControllerActionDescriptor action)
        => action.ControllerTypeInfo.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ICrudModelService<,,>));
}