using FS.TimeTracking.Api.REST.Startup;
using FS.TimeTracking.Repository.Startup;
using FS.TimeTracking.Shared.Extensions;
using FS.TimeTracking.Shared.Models.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NLog.Web;
using System;
using System.IO;
using System.Linq;

namespace FS.TimeTracking
{
    public static class Program
    {
        private const string CONFIG_BASE_NAME = "FS.TimeTracking.config";
        private const string NLOG_CONFIGURATION_FILE = CONFIG_BASE_NAME + ".nlog";

        private static readonly string _executablePath = AssemblyExtensions.GetProgramDirectory();
        private static readonly string _pathToContentRoot = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development"
            ? Directory.GetCurrentDirectory()
            : _executablePath;

        public static void Main(string[] args)
        {
            try
            {
                var options = new CommandLineOptions(args);
                if (!options.Parsed)
                    return;

                var host = CreateHostBuilder(args)
                    .UseWindowsService()
                    .UseSystemd()
                    .Build();

                if (options.GenerateOpenApiJsonFile)
                    host.GenerateOpenApiJson(options.OpenApiJsonFile);
                else
                    host.Run();
            }
            catch (Exception exception)
            {
                using var loggerFactory = NLogBuilder.ConfigureNLog(NLOG_CONFIGURATION_FILE);
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

        public static IHostBuilder CreateHostBuilder(string[] args)
            => Host.CreateDefaultBuilder(args)
                .UseContentRoot(_pathToContentRoot)
                .ConfigureAppConfiguration(builder => ConfigureServerConfiguration(builder, args))
                .ConfigureNlog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureServices(ConfigureServerServices);
                    webBuilder.Configure((builder, app) => ConfigureServerApplication(app, builder.HostingEnvironment));
                });

        private static void ConfigureServerConfiguration(IConfigurationBuilder configurationBuilder, string[] commandLineArgs)
        {
            var chainedConfigurationSource = configurationBuilder.Sources.OfType<ChainedConfigurationSource>().First();
            configurationBuilder.Sources.Clear();
            configurationBuilder
                .Add(chainedConfigurationSource)
                .AddJsonFile($"{CONFIG_BASE_NAME}.json", false, true)
                .AddJsonFile($"{CONFIG_BASE_NAME}.Development.json", true, true)
                .AddJsonFile($"{CONFIG_BASE_NAME}.User.json", true, true)
                .AddEnvironmentVariables()
                .AddCommandLine(commandLineArgs);
        }

        private static IHostBuilder ConfigureNlog(this IHostBuilder builder)
            => builder
                .ConfigureLogging(logging =>
                    logging
                        .ClearProviders()
                        .SetMinimumLevel(LogLevel.Trace)
                        .AddNLog(NLOG_CONFIGURATION_FILE)
                )
                .UseNLog();

        private static void ConfigureServerServices(WebHostBuilderContext context, IServiceCollection services)
        {
            services
                .CreateAndRegisterEnvironmentConfiguration(context.HostingEnvironment)
                .Configure<TimeTrackingConfiguration>(context.Configuration.GetSection(TimeTrackingConfiguration.CONFIGURATION_SECTION))
                .AddSingleton(serviceProvider => serviceProvider.GetRequiredService<IOptions<TimeTrackingConfiguration>>().Value)
                .RegisterApplicationServices()
                .RegisterOpenApiController()
                .RegisterRestApiController()
                .RegisterSpaStaticFiles(context.HostingEnvironment);
        }

        private static void ConfigureServerApplication(IApplicationBuilder applicationBuilder, IHostEnvironment hostEnvironment)
        {
            if (hostEnvironment.IsDevelopment())
            {
                applicationBuilder.UseDeveloperExceptionPage();
                applicationBuilder.UseCors(policy => policy.AllowAnyOrigin());
            }

            applicationBuilder
                //.UseHttpsRedirection()
                .UseRouting()
                //.UseAuthorization()
                .RegisterOpenApiRoutes()
                .RegisterRestApiRoutes()
                .RegisterSpaRoutes(hostEnvironment)
                .MigrateDatabase();
        }

        private static IServiceCollection CreateAndRegisterEnvironmentConfiguration(this IServiceCollection services, IHostEnvironment hostEnvironment)
            => services.AddSingleton(new EnvironmentConfiguration
            {
                IsDevelopment = hostEnvironment.IsDevelopment(),
                IsProduction = hostEnvironment.IsProduction(),
            });

        private static IServiceCollection RegisterSpaStaticFiles(this IServiceCollection services, IHostEnvironment hostEnvironment)
        {
            if (hostEnvironment.IsDevelopment())
                return services;

            services.AddSpaStaticFiles(configuration => configuration.RootPath = Path.Combine(_executablePath, "UI"));
            return services;
        }

        private static IApplicationBuilder RegisterSpaRoutes(this IApplicationBuilder applicationBuilder, IHostEnvironment hostEnvironment)
        {
            if (hostEnvironment.IsDevelopment())
                return applicationBuilder;

            applicationBuilder.UseSpaStaticFiles();
            applicationBuilder.UseSpa(_ => { });
            return applicationBuilder;
        }
    }
}
