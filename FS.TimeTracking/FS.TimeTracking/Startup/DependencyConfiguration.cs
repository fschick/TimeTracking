using FS.TimeTracking.Application.Services.Chart;
using FS.TimeTracking.Application.Services.MasterData;
using FS.TimeTracking.Application.Services.Shared;
using FS.TimeTracking.Application.Services.TimeTracking;
using FS.TimeTracking.Application.ValidationConverters;
using FS.TimeTracking.Repository.DbContexts;
using FS.TimeTracking.Repository.Services;
using FS.TimeTracking.Shared.DTOs.MasterData;
using FS.TimeTracking.Shared.Interfaces.Application.Services.Chart;
using FS.TimeTracking.Shared.Interfaces.Application.Services.MasterData;
using FS.TimeTracking.Shared.Interfaces.Application.Services.Shared;
using FS.TimeTracking.Shared.Interfaces.Application.Services.TimeTracking;
using FS.TimeTracking.Shared.Interfaces.Repository.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FS.TimeTracking.Startup;

internal static class DependencyConfiguration
{
    public static IServiceCollection RegisterApplicationServices(this IServiceCollection services)
    {
        //services.AddDbContextPool<TimeTrackingDbContext>(o => { });
        services.AddDbContext<TimeTrackingDbContext>();
        services.AddScoped<IRepository, Repository<TimeTrackingDbContext>>();
        services.AddSingleton<IDbExceptionService, DbExceptionService>();
        services.AddScoped<ITruncateDbService, TruncateDbService>();

        services.AddScoped<IWorkdayService, WorkdayService>();
        services.AddScoped<IInformationService, InformationService>();
        services.AddScoped<ITestDataService, TestDataService>();
        services.AddScoped<ITypeaheadService, TypeaheadService>();
        services.AddScoped<IValidationDescriptionService, ValidationDescriptionService<ActivityDto, RequiredValidationConverter>>();

        services.AddScoped<ITimeSheetService, TimeSheetService>();
        services.AddScoped<ICustomerChartService, CustomerChartService>();
        services.AddScoped<IOrderChartService, OrderChartService>();
        services.AddScoped<IActivityChartService, ActivityChartService>();
        services.AddScoped<IProjectChartService, ProjectChartService>();
        services.AddScoped<IIssueChartService, IssueChartService>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IProjectService, ProjectService>();
        services.AddScoped<IActivityService, ActivityService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IHolidayService, HolidayService>();
        services.AddScoped<ISettingService, SettingService>();

#if DEBUG
        services.AddScoped<IDebugService, DebugService>();
#endif
        return services;
    }
}