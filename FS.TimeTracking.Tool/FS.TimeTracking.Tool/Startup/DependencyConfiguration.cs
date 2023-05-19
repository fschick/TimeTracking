using FS.TimeTracking.Application.Services.Administration;
using FS.TimeTracking.Application.Services.Shared;
using FS.TimeTracking.Application.Startup;
using FS.TimeTracking.Core.Interfaces.Application.Services.Administration;
using FS.TimeTracking.Core.Interfaces.Application.Services.Shared;
using FS.TimeTracking.Core.Interfaces.Repository.Services.Database;
using FS.TimeTracking.Core.Models.Configuration;
using FS.TimeTracking.Repository.DbContexts;
using FS.TimeTracking.Repository.Services;
using FS.TimeTracking.Tool.AutoMapper.Imports;
using FS.TimeTracking.Tool.DbContexts.Imports;
using FS.TimeTracking.Tool.Interfaces.Import;
using FS.TimeTracking.Tool.Models.Configurations;
using FS.TimeTracking.Tool.Services.Imports;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;

namespace FS.TimeTracking.Tool.Startup;

internal static class DependencyConfiguration
{
    public static IServiceCollection RegisterApplicationServices(this IServiceCollection services, CommandLineOptions commandLineOptions)
        => services
            .AddSingleton(commandLineOptions)
            .AddLogging(c => c.AddConsole())
            .RegisterConfiguration(commandLineOptions)
            .RegisterTimeTrackingAutoMapper()
            .RegisterKimaiV1AutoMapper()
            .AddDbContext<TimeTrackingDbContext>()
            .AddDbContext<KimaiV1DbContext>()
            .AddScoped(CreateTimeTrackingImportDbContext)
            .AddScoped<IDbRepository, DbRepository<TimeTrackingDbContext>>()
            .AddScoped<IKimaiV1Repository, KimaiV1Repository>()
            .AddScoped<ITimeTrackingImportRepository, TimeTrackingImportRepository>()
            .AddScoped<IWorkdayService, WorkdayService>()
            .AddScoped<ISettingApiService, SettingService>()
            .AddScoped<IKimaiV1ImportService, KimaiV1ImportService>()
            .AddScoped<ITimeTrackingImportService, TimeTrackingImportService>()
            .AddScoped<IDbTruncateService, DbTruncateService>()
            .AddScoped<IDbMigrationService, DbMigrationService>();

    private static IServiceCollection RegisterConfiguration(this IServiceCollection services, CommandLineOptions commandLineOptions)
        => services
            .AddSingleton(CreateEnvironmentConfiguration())
            .AddSingleton(commandLineOptions.CreateTimeTrackingOptions())
            .AddSingleton(commandLineOptions.CreateKimaiV1ImportOptions())
            .AddSingleton(commandLineOptions.CreateTimeTrackingImportOptions());

    private static IServiceCollection RegisterKimaiV1AutoMapper(this IServiceCollection services)
        => services.AddAutoMapper(typeof(KimaiV1AutoMapper));

    private static IOptions<TimeTrackingConfiguration> CreateTimeTrackingOptions(this CommandLineOptions commandLineOptions)
    {
        var timeTrackingConfiguration = new TimeTrackingConfiguration
        {
            Database =
            {
                ConnectionString = commandLineOptions.DestinationConnectionString,
                Type = commandLineOptions.DestinationDatabaseType
            }
        };

        return Options.Create(timeTrackingConfiguration);
    }

    private static IOptions<KimaiV1ImportConfiguration> CreateKimaiV1ImportOptions(this CommandLineOptions commandLineOptions)
    {
        var timeTrackingConfiguration = new KimaiV1ImportConfiguration
        {
            SourceConnectionString = commandLineOptions.SourceConnectionString,
            SourceDatabaseType = commandLineOptions.SourceDatabaseType,
            TablePrefix = commandLineOptions.SourceTablePrefix,
            TruncateBeforeImport = commandLineOptions.TruncateBeforeImport,
        };

        return Options.Create(timeTrackingConfiguration);
    }

    private static IOptions<TimeTrackingImportConfiguration> CreateTimeTrackingImportOptions(this CommandLineOptions commandLineOptions)
    {
        var timeTrackingImportConfiguration = new TimeTrackingImportConfiguration
        {
            SourceConnectionString = commandLineOptions.SourceConnectionString,
            SourceDatabaseType = commandLineOptions.SourceDatabaseType,
            TruncateBeforeImport = commandLineOptions.TruncateBeforeImport,
        };

        return Options.Create(timeTrackingImportConfiguration);
    }

    private static TimeTrackingImportDbContext CreateTimeTrackingImportDbContext(IServiceProvider services)
    {
        var loggerFactory = services.GetRequiredService<ILoggerFactory>();
        var timeTrackingImportConfiguration = services.GetRequiredService<IOptions<TimeTrackingImportConfiguration>>();
        var environmentConfiguration = services.GetRequiredService<EnvironmentConfiguration>();

        var timeTrackingConfiguration = new TimeTrackingConfiguration
        {
            Database =
            {
                ConnectionString = timeTrackingImportConfiguration.Value.SourceConnectionString,
                Type = timeTrackingImportConfiguration.Value.SourceDatabaseType,
            }
        };

        return new TimeTrackingImportDbContext(loggerFactory, Options.Create(timeTrackingConfiguration), environmentConfiguration);
    }

    private static EnvironmentConfiguration CreateEnvironmentConfiguration()
        => new EnvironmentConfiguration { IsDevelopment = true, IsProduction = false };
}