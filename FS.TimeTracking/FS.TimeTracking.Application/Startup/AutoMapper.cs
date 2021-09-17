using FS.TimeTracking.Application.AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace FS.TimeTracking.Application.Startup
{
    internal static class AutoMapper
    {
        public static IServiceCollection RegisterTimeTrackingAutoMapper(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(TimeTrackingAutoMapper));
            return services;
        }
    }
}
