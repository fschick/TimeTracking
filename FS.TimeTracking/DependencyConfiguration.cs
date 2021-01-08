using FS.TimeTracking.Application.Services;
using FS.TimeTracking.Shared.Interfaces.Application;
using Microsoft.Extensions.DependencyInjection;

namespace FS.TimeTracking
{
    public static class DependencyConfiguration
    {
        public static IServiceCollection RegisterApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IInformationService, InformationService>();
#if DEBUG
            services.AddScoped<IDevTestService, DevTestService>();
#endif
            return services;
        }
    }
}