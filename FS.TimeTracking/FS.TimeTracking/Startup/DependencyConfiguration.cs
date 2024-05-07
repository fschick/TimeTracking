using FS.Keycloak.RestApiClient.Api;
using FS.Keycloak.RestApiClient.Authentication.Client;
using FS.TimeTracking.Abstractions.DTOs.MasterData;
using FS.TimeTracking.Application.BackgroundServices;
using FS.TimeTracking.Application.Services.Administration;
using FS.TimeTracking.Application.Services.Chart;
using FS.TimeTracking.Application.Services.MasterData;
using FS.TimeTracking.Application.Services.Reporting;
using FS.TimeTracking.Application.Services.Shared;
using FS.TimeTracking.Application.Services.TimeTracking;
using FS.TimeTracking.Application.ValidationConverters;
using FS.TimeTracking.Core.Interfaces.Application.Services.Administration;
using FS.TimeTracking.Core.Interfaces.Application.Services.Chart;
using FS.TimeTracking.Core.Interfaces.Application.Services.MasterData;
using FS.TimeTracking.Core.Interfaces.Application.Services.Reporting;
using FS.TimeTracking.Core.Interfaces.Application.Services.Shared;
using FS.TimeTracking.Core.Interfaces.Application.Services.TimeTracking;
using FS.TimeTracking.Core.Interfaces.Repository.Services.Administration;
using FS.TimeTracking.Core.Interfaces.Repository.Services.Database;
using FS.TimeTracking.Keycloak.Services;
using FS.TimeTracking.Keycloak.Services.Repository;
using FS.TimeTracking.Repository.DbContexts;
using FS.TimeTracking.Repository.Services;
using Microsoft.Extensions.DependencyInjection;
using ApiClientFactory = FS.Keycloak.RestApiClient.ClientFactory.ApiClientFactory;

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

        services.AddScoped<IKeycloakRepository, KeycloakRepository>();
        services.AddScoped<IKeycloakDeploymentService, KeycloakDeploymentService>();

        services.AddScoped<AuthenticationHttpClient>(AuthenticationHttpClientFactory.Create);
        services.AddScoped<IRealmsAdminApi>(sp => ApiClientFactory.Create<RealmsAdminApi>(sp.GetRequiredService<AuthenticationHttpClient>()));
        services.AddScoped<IClientsApi>(sp => ApiClientFactory.Create<ClientsApi>(sp.GetRequiredService<AuthenticationHttpClient>()));
        services.AddScoped<IClientScopesApi>(sp => ApiClientFactory.Create<ClientScopesApi>(sp.GetRequiredService<AuthenticationHttpClient>()));
        services.AddScoped<IRolesApi>(sp => ApiClientFactory.Create<RolesApi>(sp.GetRequiredService<AuthenticationHttpClient>()));
        services.AddScoped<IUsersApi>(sp => ApiClientFactory.Create<UsersApi>(sp.GetRequiredService<AuthenticationHttpClient>()));
        services.AddScoped<IClientRoleMappingsApi>(sp => ApiClientFactory.Create<ClientRoleMappingsApi>(sp.GetRequiredService<AuthenticationHttpClient>()));
        services.AddScoped<IComponentApi>(sp => ApiClientFactory.Create<ComponentApi>(sp.GetRequiredService<AuthenticationHttpClient>()));

        services.AddScoped<IWorkdayService, WorkdayService>();
        services.AddScoped<IAuthorizationService, AuthorizationService>();
        services.AddScoped<IInformationApiService, InformationService>();
        services.AddScoped<IMaintenanceService, MaintenanceService>();
        services.AddScoped<IMaintenanceApiService>(sp => sp.GetRequiredService<IMaintenanceService>());
        services.AddScoped<IFilterFactory, FilterFactory>();
        services.AddScoped<ITypeaheadApiService, TypeaheadService>();
        services.AddScoped<IValidationDescriptionApiService, ValidationDescriptionService<ActivityDto, RequiredValidationConverter>>();

        services.AddScoped<IOrderChartService, OrderChartService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IUserApiService>(sp => sp.GetRequiredService<IUserService>());

        services.AddScoped<ITimeSheetApiService, TimeSheetService>();
        services.AddScoped<ICustomerChartApiService, CustomerChartService>();
        services.AddScoped<IOrderChartApiService>(sp => sp.GetRequiredService<IOrderChartService>());
        services.AddScoped<IActivityChartApiService, ActivityChartService>();
        services.AddScoped<IProjectChartApiService, ProjectChartService>();
        services.AddScoped<IIssueChartApiService, IssueChartService>();
        services.AddScoped<IActivityReportApiService, ActivityReportService>();
        services.AddHttpClient<IActivityReportApiService, ActivityReportService>();
        services.AddScoped<ICustomerApiService, CustomerService>();
        services.AddScoped<IProjectApiService, ProjectService>();
        services.AddScoped<IActivityApiService, ActivityService>();
        services.AddScoped<IOrderApiService, OrderService>();
        services.AddScoped<IHolidayApiService, HolidayService>();
        services.AddScoped<ISettingApiService, SettingService>();

        services.AddHostedService<DataResetBackgroundService>();

#if DEBUG
        services.AddScoped<IDebugApiService, DebugService>();
#endif
        return services;
    }
}