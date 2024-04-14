
using AutoMapper.Extensions.ExpressionMapping;
using FS.TimeTracking.Application.AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace FS.TimeTracking.Application.Startup;

internal static class AutoMapperStartup
{
    public static IServiceCollection RegisterTimeTrackingAutoMapper(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg => cfg.AddExpressionMapping(), typeof(TimeTrackingAutoMapper).Assembly);
        return services;
    }
}