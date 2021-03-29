using FS.TimeTracking.Application.AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace FS.TimeTracking.Application.Startup
{
    internal static class AutoMapper
    {
        public static IServiceCollection RegisterAutoMapper(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(TimeTrackingProfile));
            return services;
        }
    }
}
