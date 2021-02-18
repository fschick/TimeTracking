using FS.TimeTracking.Api.REST.Filters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;

namespace FS.TimeTracking.Api.REST.Startup
{
    internal static class RestApi
    {
        public static IApplicationBuilder RegisterRestApiRoutes(this IApplicationBuilder applicationBuilder)
            => applicationBuilder
                .UseEndpoints(endpoints => endpoints.MapControllers());

        public static IServiceCollection RegisterRestApiController(this IServiceCollection services)
        {
            services
                .AddControllers(o =>
                {
                    o.OutputFormatters.RemoveType<StringOutputFormatter>();
                    o.Filters.Add<RequestIdHeaderFilter>();
                })
                .AddNewtonsoftJson();

            return services;
        }
    }
}
