using FS.TimeTracking.Application.FilterExpressionInterceptors;
using Microsoft.Extensions.DependencyInjection;
using Plainquire.Filter;

namespace FS.TimeTracking.Application.Startup;

internal static class FilterExpressionInterceptorStartup
{
    public static IServiceCollection RegisterFilterExpressionInterceptor(this IServiceCollection services)
    {
        IFilterInterceptor.Default = new DateTimeOffsetInterceptor();
        return services;
    }
}