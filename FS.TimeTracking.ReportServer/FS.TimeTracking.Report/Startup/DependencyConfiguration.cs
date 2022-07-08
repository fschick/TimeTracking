using FS.TimeTracking.Report.Application.Services.Report;
using FS.TimeTracking.Report.Application.Services.Shared;
using FS.TimeTracking.Report.Core.Interfaces.Application.Services.Report;
using FS.TimeTracking.Report.Core.Interfaces.Application.Services.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace FS.TimeTracking.Report.Startup;

internal static class DependencyConfiguration
{
    public static IServiceCollection RegisterApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IInformationService, InformationService>();
        services.AddScoped<IActivityReportService, ActivityReportService>();
        return services;
    }
}