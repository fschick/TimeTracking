using FS.TimeTracking.Application.Services.Shared;
using FS.TimeTracking.Application.Startup;
using FS.TimeTracking.Repository.DbContexts;
using FS.TimeTracking.Repository.Services;
using FS.TimeTracking.Shared.Interfaces.Application.Services.Shared;
using FS.TimeTracking.Shared.Interfaces.Repository.Services;
using FS.TimeTracking.Shared.Models.Configuration;
using FS.TimeTracking.Tool.AutoMapper.Imports;
using FS.TimeTracking.Tool.DbContexts.Imports;
using FS.TimeTracking.Tool.Interfaces.Import;
using FS.TimeTracking.Tool.Models.Configurations;
using FS.TimeTracking.Tool.Services.Imports;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FS.TimeTracking.Tool
{
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
                .AddScoped<IRepository, Repository<TimeTrackingDbContext>>()
                .AddScoped<IKimaiV1Repository, KimaiV1Repository>()
                .AddScoped<IWorkDaysService, WorkDaysService>()
                .AddScoped<ITestDataService, TestDataService>()
                .AddScoped<IKimaiV1ImportService, KimaiV1ImportService>();

        private static IServiceCollection RegisterConfiguration(this IServiceCollection services, CommandLineOptions commandLineOptions)
            => services
                .AddSingleton(CreateEnvironmentConfiguration())
                .AddSingleton(commandLineOptions.CreateTimeTrackingOptions())
                .AddSingleton(commandLineOptions.CreateKimaiV1ImportConfiguration());

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

        private static IOptions<KimaiV1ImportConfiguration> CreateKimaiV1ImportConfiguration(this CommandLineOptions commandLineOptions)
        {
            var timeTrackingConfiguration = new KimaiV1ImportConfiguration
            {
                ConnectionString = commandLineOptions.SourceConnectionString,
                DatabaseType = commandLineOptions.SourceDatabaseType,
                TablePrefix = commandLineOptions.SourceTablePrefix,
                TruncateBeforeImport = commandLineOptions.TruncateBeforeImport,
            };

            return Options.Create(timeTrackingConfiguration);
        }

        private static EnvironmentConfiguration CreateEnvironmentConfiguration()
            => new EnvironmentConfiguration { IsDevelopment = true, IsProduction = false };
    }
}