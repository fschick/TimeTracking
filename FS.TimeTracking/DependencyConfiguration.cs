using FS.TimeTracking.Application.ModelConverters;
using FS.TimeTracking.Application.Services;
using FS.TimeTracking.Application.ValidationConverters;
using FS.TimeTracking.Repository.DbContexts;
using FS.TimeTracking.Repository.Services;
using FS.TimeTracking.Shared.DTOs.TimeTracking;
using FS.TimeTracking.Shared.Interfaces.Application.Converters;
using FS.TimeTracking.Shared.Interfaces.Application.Services;
using FS.TimeTracking.Shared.Interfaces.Services;
using FS.TimeTracking.Shared.Models.TimeTracking;
using Microsoft.Extensions.DependencyInjection;

namespace FS.TimeTracking
{
    internal static class DependencyConfiguration
    {
        public static IServiceCollection RegisterApplicationServices(this IServiceCollection services)
        {
            services.AddSingleton<IModelConverter<Activity, ActivityDto>, ActivityConverter>();
            services.AddSingleton<IModelConverter<Customer, CustomerDto>, CustomerConverter>();
            services.AddSingleton<IModelConverter<Project, ProjectDto>, ProjectConverter>();
            services.AddSingleton<IModelConverter<TimeSheet, TimeSheetDto>, TimeSheetConverter>();

            //services.AddDbContextPool<TimeTrackingDbContext>(o => { });
            services.AddDbContext<TimeTrackingDbContext>();
            services.AddScoped<IRepository, Repository<TimeTrackingDbContext>>();
            services.AddSingleton<IDbExceptionService, DbExceptionService>();

            services.AddScoped<IInformationService, InformationService>();
            services.AddScoped<IActivityService, ActivityService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IProjectService, ProjectService>();
            services.AddScoped<ITimeSheetService, TimeSheetService>();
            services.AddScoped<IValidationDescriptionService, ValidationDescriptionService<ActivityDto, RequiredValidationConverter>>();
#if DEBUG
            services.AddScoped<IDebugService, DebugService>();
#endif
            return services;
        }
    }
}