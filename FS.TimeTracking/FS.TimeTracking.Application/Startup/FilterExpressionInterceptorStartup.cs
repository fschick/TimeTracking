using FS.FilterExpressionCreator.Filters;
using FS.TimeTracking.Application.FilterExpressionInterceptors;
using Microsoft.Extensions.DependencyInjection;

namespace FS.TimeTracking.Application.Startup;

internal static class FilterExpressionInterceptorStartup
{
    public static IServiceCollection RegisterFilterExpressionInterceptor(this IServiceCollection services)
    {
        EntityFilter.DefaultInterceptor = new DateTimeOffsetInterceptor();
        return services;
    }
}