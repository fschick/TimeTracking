using FS.TimeTracking.Api.REST.Startup;
using FS.TimeTracking.Application.Startup;
using FS.TimeTracking.Repository.Startup;
using FS.TimeTracking.Shared.Extensions;
using FS.TimeTracking.Shared.Models.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NLog.Web;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FS.TimeTracking
{
    internal static class Program
    {
        private const string CONFIG_FOLDER = "config";
        private const string CONFIG_BASE_NAME = "FS.TimeTracking.config";
        private const string NLOG_CONFIGURATION_FILE = CONFIG_BASE_NAME + ".nlog";

        private static readonly string _executablePath = AssemblyExtensions.GetProgramDirectory();
        private static readonly string _pathToContentRoot = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development"
            ? Directory.GetCurrentDirectory()
            : _executablePath;

        public static async Task Main(string[] args)
        {
            try
            {
                var options = new CommandLineOptions(args);
                if (!options.Parsed)
                    return;

                using var host = CreateHostBuilder(args)
                    .UseWindowsService()
                    .UseSystemd()
                    .Build();

                if (options.GenerateOpenApiSpecFile)
                    host.GenerateOpenApiSpec(options.OpenApiSpecFile);
                else if (options.GenerateValidationSpecFile)
                    await host.GenerateValidationSpec(options.ValidationSpecFile);
                else
                    await host.RunAsync();
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

        public static IHostBuilder CreateHostBuilder(string[] args)
            => CreateHostBuilderInternal(args, builder => builder.AddConfigurationFromEnvironment(args));

        internal static IHostBuilder CreateHostBuilderInternal(TimeTrackingConfiguration configuration)
            => CreateHostBuilderInternal(Array.Empty<string>(), builder => builder.AddConfigurationFromBluePrint(configuration));

        private static IHostBuilder CreateHostBuilderInternal(string[] args, Action<IConfigurationBuilder> configurationBuilder)
            => Host.CreateDefaultBuilder(args)
                .UseContentRoot(_pathToContentRoot)
                .ConfigureAppConfiguration(builder =>
                {
                    builder.ClearConfiguration();
                    configurationBuilder(builder);
                })
                .ConfigureNlog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureServices(ConfigureServerServices);
                    webBuilder.Configure((builder, app) => ConfigureServerApplication(app, builder.HostingEnvironment));
                });

        private static void ClearConfiguration(this IConfigurationBuilder configurationBuilder)
        {
            var chainedConfigurationSource = configurationBuilder.Sources.OfType<ChainedConfigurationSource>().First();
            configurationBuilder.Sources.Clear();
            configurationBuilder
                .Add(chainedConfigurationSource);
        }

        private static void AddConfigurationFromEnvironment(this IConfigurationBuilder configurationBuilder, string[] commandLineArgs)
        {
            configurationBuilder
                .AddJsonFile($"{Path.Combine(CONFIG_FOLDER, CONFIG_BASE_NAME)}.json", false, true)
                    .AddJsonFile($"{Path.Combine(CONFIG_FOLDER, CONFIG_BASE_NAME)}.Development.json", true, true)
                    .AddEnvironmentVariables()
                    .AddCommandLine(commandLineArgs);
        }

        private static void AddConfigurationFromBluePrint(this IConfigurationBuilder configurationBuilder, TimeTrackingConfiguration configuration)
        {
            var json = JsonConvert.SerializeObject(new { TimeTracking = configuration });
            using var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            var configurationRoot = new ConfigurationBuilder().AddJsonStream(memoryStream).Build();
            configurationBuilder.AddConfiguration(configurationRoot);
        }

        private static IHostBuilder ConfigureNlog(this IHostBuilder builder)
            => builder
                .ConfigureLogging(logging =>
                    logging
                        .ClearProviders()
                        .SetMinimumLevel(LogLevel.Trace)
                        .AddNLog(Path.Combine(CONFIG_FOLDER, NLOG_CONFIGURATION_FILE))
                )
                .UseNLog();

        private static void ConfigureServerServices(WebHostBuilderContext context, IServiceCollection services)
        {
            //FS.TimeTracking.Application.Startup.AutoMapper.RegisterAutoMapper();
            services
                .CreateAndRegisterEnvironmentConfiguration(context.HostingEnvironment)
                .Configure<TimeTrackingConfiguration>(context.Configuration.GetSection(TimeTrackingConfiguration.CONFIGURATION_SECTION))
                .RegisterTimeTrackingAutoMapper()
                .RegisterFilterExpressionInterceptor()
                .RegisterApplicationServices()
                .RegisterOpenApiController()
                .RegisterRestApiController()
                .RegisterSpaStaticFiles(context.HostingEnvironment);
        }

        private static void ConfigureServerApplication(IApplicationBuilder applicationBuilder, IHostEnvironment hostEnvironment)
        {
            if (hostEnvironment.IsDevelopment())
            {
#if DEBUG
                applicationBuilder.UseDeveloperExceptionPage();
                applicationBuilder.UseCors(policy => policy
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                );
#endif
            }
            else
            {
                applicationBuilder.UseHttpsRedirection();
            }

            applicationBuilder
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
            //if (hostEnvironment.IsProduction())
            //    services.AddSpaStaticFiles(configuration => configuration.RootPath = Path.Combine(_executablePath, "UI"));

            services
                .AddSpaStaticFiles(configuration =>
                    configuration.RootPath = hostEnvironment.IsProduction()
                        ? Path.Combine(_executablePath, "UI")
                        : "../FS.TimeTracking.UI.Angular/dist/TimeTracking"
                );

            return services;
        }

        private static IApplicationBuilder RegisterSpaRoutes(this IApplicationBuilder applicationBuilder, IHostEnvironment hostEnvironment)
        {
            //if (hostEnvironment.IsProduction())
            //{
            //    applicationBuilder.UseSpaStaticFiles();
            //    applicationBuilder.UseSpa(_ => { });
            //}
            //else
            //{
            //    applicationBuilder.UseSpa(c => c.UseProxyToSpaDevelopmentServer("http://localhost:4200/"));
            //}

            applicationBuilder.UseSpaStaticFiles();
            applicationBuilder.UseSpa(_ => { });

            return applicationBuilder;
        }
    }
}
