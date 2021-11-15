using FS.TimeTracking.Api.REST.Startup;
using FS.TimeTracking.Startup;
using Microsoft.AspNetCore.Builder;
using NLog.Web;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;

namespace FS.TimeTracking
{
    internal class Program
    {
        internal const string WEB_UI_FOLDER = "webui";
        internal const string CONFIG_FOLDER = "config";
        internal const string CONFIG_BASE_NAME = "FS.TimeTracking.config";
        internal const string NLOG_CONFIGURATION_FILE = CONFIG_BASE_NAME + ".nlog";

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
                    await webApp.RunAsync();
            }
            catch (Exception exception)
            {
                using var loggerFactory = NLogBuilder.ConfigureNLog(Path.Combine(CONFIG_FOLDER, NLOG_CONFIGURATION_FILE));
                loggerFactory
                    .GetCurrentClassLogger()
                    .Error(exception, "Program stopped due to an exception");
                throw;
            }
            finally
            {
                NLog.LogManager.Shutdown();
            }
        }

        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Required by EF")]
        public static WebApplicationBuilder CreateHostBuilder(string[] args)
            => TimeTrackingWebApp.CreateWebApplicationBuilder(args);
    }
}
