using FS.TimeTracking.Api.REST.Filters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace FS.TimeTracking.Api.REST.Startup;

internal static class RestApi
{
    public static WebApplication RegisterRestApiRoutes(this WebApplication webApplication)
    {
        webApplication
            .UseEndpoints(endpoints => endpoints.MapControllers());
        return webApplication;
    }

    public static IServiceCollection RegisterRestApiController(this IServiceCollection services)
    {
        services
            .AddControllers(o =>
            {
                o.OutputFormatters.RemoveType<StringOutputFormatter>();
                o.Filters.Add<AddRequestIdToHeaderFilter>();
                o.Filters.Add<ExceptionToHttpResultFilter>();
            })
            .AddFilterExpressionCreators()
            .AddNewtonsoftJson(opts =>
            {
                var camelCase = new CamelCaseNamingStrategy();
                opts.SerializerSettings.Converters.Add(new StringEnumConverter(camelCase));
            });

        return services;
    }
}