using FS.TimeTracking.Abstractions.Constants;
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
using System.Linq;

namespace FS.TimeTracking.Api.REST.Startup;

internal static class RestApiStartup
{
    public static WebApplication RegisterRestApiRoutes(this WebApplication webApplication)
    {
        webApplication
            .UseEndpoints(endpoints =>
            {
                var controllerBuilder = endpoints.MapControllers();

                var configuration = webApplication.Services.GetRequiredService<IOptions<TimeTrackingConfiguration>>().Value;
                if (!configuration.Features.Authorization)
                    controllerBuilder.AllowAnonymous();
            });
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
            .AddFilterExpressionCreators()
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
                foreach (var policyName in PermissionNames.CrudServicePolicyPermissions)
                    options.AddPolicy(policyName, CreateCrudServicePolicy(policyName));
            });

        return services;
    }

    private static Action<AuthorizationPolicyBuilder> CreateCrudServicePolicy(string policyName)
        => policy =>
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
                    case nameof(ICrudModelService<int, int, int>.Get):
                    case nameof(ICrudModelService<int, int, int>.GetGridFiltered):
                    case nameof(ICrudModelService<int, int, int>.GetGridItem):
                        return context.User.IsInRole($"{policyName}-{ScopeNames.VIEW}");
                    case nameof(ICrudModelService<int, int, int>.Create):
                    case nameof(ICrudModelService<int, int, int>.Update):
                    case nameof(ICrudModelService<int, int, int>.Delete):
                        return context.User.IsInRole($"{policyName}-{ScopeNames.MANAGE}");
                    default:
                        return true;
                }
            });
        };

    private static bool IsCrudModelService(this ControllerActionDescriptor action)
        => action.ControllerTypeInfo.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ICrudModelService<,,>));
}