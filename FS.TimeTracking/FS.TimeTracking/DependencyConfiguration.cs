using FS.TimeTracking.Application.Services;
using FS.TimeTracking.Application.ValidationConverters;
using FS.TimeTracking.Repository.DbContexts;
using FS.TimeTracking.Repository.Services;
using FS.TimeTracking.Shared.DTOs.TimeTracking;
using FS.TimeTracking.Shared.Interfaces.Application.Services;
using FS.TimeTracking.Shared.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FS.TimeTracking
{
    internal static class DependencyConfiguration
    {
        public static IServiceCollection RegisterApplicationServices(this IServiceCollection services)
        {
            //services.AddDbContextPool<TimeTrackingDbContext>(o => { });
            services.AddDbContext<TimeTrackingDbContext>();
            services.AddScoped<IRepository, Repository<TimeTrackingDbContext>>();
            services.AddSingleton<IDbExceptionService, DbExceptionService>();
            services.AddScoped<ITruncateDbService, TruncateDbService>();

            services.AddSingleton<IWorkDaysService, WorkDaysService>();
            services.AddScoped<IInformationService, InformationService>();
            services.AddScoped<IActivityService, ActivityService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IProjectService, ProjectService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<ITimeSheetService, TimeSheetService>();
            services.AddScoped<ITestDataService, TestDataService>();
            services.AddScoped<ITypeaheadService, TypeaheadService>();
            services.AddScoped<IValidationDescriptionService, ValidationDescriptionService<ActivityDto, RequiredValidationConverter>>();
#if DEBUG
            services.AddScoped<IDebugService, DebugService>();
#endif
            return services;
        }
    }
}