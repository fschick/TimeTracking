using FS.Keycloak.RestApiClient.Api;
using FS.Keycloak.RestApiClient.Client;
using FS.TimeTracking.Abstractions.DTOs.MasterData;
using FS.TimeTracking.Application.Services.Chart;
using FS.TimeTracking.Application.Services.MasterData;
using FS.TimeTracking.Application.Services.Reporting;
using FS.TimeTracking.Application.Services.Shared;
using FS.TimeTracking.Application.Services.TimeTracking;
using FS.TimeTracking.Application.ValidationConverters;
using FS.TimeTracking.Core.Interfaces.Application.Services.Authorization;
using FS.TimeTracking.Core.Interfaces.Application.Services.Chart;
using FS.TimeTracking.Core.Interfaces.Application.Services.MasterData;
using FS.TimeTracking.Core.Interfaces.Application.Services.Reporting;
using FS.TimeTracking.Core.Interfaces.Application.Services.Shared;
using FS.TimeTracking.Core.Interfaces.Application.Services.TimeTracking;
using FS.TimeTracking.Core.Interfaces.Repository.Services.Administration;
using FS.TimeTracking.Core.Interfaces.Repository.Services.Database;
using FS.TimeTracking.Keycloak.Services;
using FS.TimeTracking.Repository.DbContexts;
using FS.TimeTracking.Repository.Services.Administration;
using FS.TimeTracking.Repository.Services.Database;
using Microsoft.Extensions.DependencyInjection;

namespace FS.TimeTracking.Startup;

internal static class DependencyConfiguration
{
    public static IServiceCollection RegisterApplicationServices(this IServiceCollection services)
    {
        //services.AddDbContextPool<TimeTrackingDbContext>(o => { });
        services.AddDbContext<TimeTrackingDbContext>();
        services.AddScoped<IDbRepository, DbRepository<TimeTrackingDbContext>>();
        services.AddSingleton<IDbExceptionService, DbExceptionService>();
        services.AddScoped<IDbTruncateService, DbTruncateService>();

        services.AddScoped<KeycloakHttpClient>(KeycloakHttpClientFactory.Create);
        services.AddScoped<IRealmsAdminApi>(sp => ApiClientFactory.Create<RealmsAdminApi>(sp.GetRequiredService<KeycloakHttpClient>()));
        services.AddScoped<IRealmAdminApi>(sp => ApiClientFactory.Create<RealmAdminApi>(sp.GetRequiredService<KeycloakHttpClient>()));
        services.AddScoped<IClientsApi>(sp => ApiClientFactory.Create<ClientsApi>(sp.GetRequiredService<KeycloakHttpClient>()));
        services.AddScoped<IClientApi>(sp => ApiClientFactory.Create<ClientApi>(sp.GetRequiredService<KeycloakHttpClient>()));
        services.AddScoped<IClientScopesApi>(sp => ApiClientFactory.Create<ClientScopesApi>(sp.GetRequiredService<KeycloakHttpClient>()));
        services.AddScoped<IRoleContainerApi>(sp => ApiClientFactory.Create<RoleContainerApi>(sp.GetRequiredService<KeycloakHttpClient>()));
        services.AddScoped<IUsersApi>(sp => ApiClientFactory.Create<UsersApi>(sp.GetRequiredService<KeycloakHttpClient>()));
        services.AddScoped<IClientRoleMappingsApi>(sp => ApiClientFactory.Create<ClientRoleMappingsApi>(sp.GetRequiredService<KeycloakHttpClient>()));
        services.AddScoped<IKeycloakRepository, KeycloakRepository>();
        services.AddScoped<IKeycloakDeploymentService, KeycloakDeploymentService>();

        services.AddScoped<IWorkdayService, WorkdayService>();
        services.AddScoped<IAuthorizationService, AuthorizationService>();
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
        services.AddScoped<IActivityReportService, ActivityReportService>();
        services.AddHttpClient<IActivityReportService, ActivityReportService>();
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