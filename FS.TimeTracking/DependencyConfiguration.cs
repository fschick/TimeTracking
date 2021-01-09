using FS.TimeTracking.Application.Services;
using FS.TimeTracking.Repository.DbContexts;
using FS.TimeTracking.Repository.Repositories;
using FS.TimeTracking.Shared.Interfaces.Application;
using FS.TimeTracking.Shared.Interfaces.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace FS.TimeTracking
{
    public static class DependencyConfiguration
    {
        public static IServiceCollection RegisterApplicationServices(this IServiceCollection services)
        {
            //services.AddDbContextPool<TimeTrackingDbContext>(o => { });
            services.AddDbContext<TimeTrackingDbContext>();
            services.AddScoped<IRepository, Repository<TimeTrackingDbContext>>();
            services.AddScoped<IInformationService, InformationService>();
#if DEBUG
            services.AddScoped<IDevTestService, DevTestService>();
#endif
            return services;
        }
    }
}