using FS.TimeTracking.Api.REST.Startup;
using FS.TimeTracking.Core.Models.Configuration;
using FS.TimeTracking.Keycloak.Startup;
using FS.TimeTracking.Repository.Startup;
using FS.TimeTracking.Startup;
using Microsoft.AspNetCore.Builder;
using NLog;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;

namespace FS.TimeTracking;

internal class Program
{
    public static async Task Main(string[] args)
    {
        try
        {
            var options = new CommandLineOptions(args);
            if (!options.Parsed)
                return;

            await using var webApp = TimeTrackingWebApp.Create(args);

            if (options.GenerateOpenApiSpecFile)
                webApp.GenerateOpenApiSpec(options.OpenApiSpecFile);
            else if (options.GenerateValidationSpecFile)
                await webApp.GenerateValidationSpec(options.ValidationSpecFile);
            else
            {
                await webApp.MigrateDatabase();
                await webApp.CreateRealmIfNotExists();
                await webApp.RunAsync();
            }
        }
        catch (Exception exception)
        {
            var loggerConfiguration = Path.Combine(TimeTrackingConfiguration.CONFIG_FOLDER, TimeTrackingConfiguration.NLOG_CONFIGURATION_FILE);
            var logger = LogManager
                .Setup()
                .LoadConfigurationFromFile(loggerConfiguration)
                .LogFactory
                .GetCurrentClassLogger();

            logger.Error(exception, "Program stopped due to an exception");

            throw;
        }
        finally
        {
            LogManager.Shutdown();
        }
    }

    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Required by EF")]
    public static WebApplicationBuilder CreateHostBuilder(string[] args)
        => TimeTrackingWebApp.CreateWebApplicationBuilder(args);
}