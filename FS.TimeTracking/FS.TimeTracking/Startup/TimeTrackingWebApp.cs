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
using NLog.Web;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

namespace FS.TimeTracking.Startup
{
    internal static class TimeTrackingWebApp
    {
        private static readonly string _executablePath = AssemblyExtensions.GetProgramDirectory();
        private static readonly string _pathToContentRoot = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development"
            ? Directory.GetCurrentDirectory()
            : _executablePath;

        internal static WebApplication Create(string[] args)
        {
            var webAppBuilder = CreateWebApplicationBuilder(args);
            var webApp = webAppBuilder.Build();
            webApp.ConfigureServerApplication();
            return webApp;
        }

        internal static WebApplicationBuilder CreateWebApplicationBuilder(string[] args)
        {
            var options = new WebApplicationOptions { Args = args, ContentRootPath = _pathToContentRoot };
            var webAppBuilder = WebApplication.CreateBuilder(options);

            webAppBuilder.Configuration.ClearConfiguration();
            webAppBuilder.Configuration.AddConfigurationFromEnvironment(args);

            webAppBuilder.ConfigureNlog();
            webAppBuilder.WebHost.ConfigureServices(ConfigureServerServices);

            webAppBuilder.Host
                .UseWindowsService()
                .UseSystemd();

            return webAppBuilder;
        }

        private static void ClearConfiguration(this IConfigurationBuilder configurationBuilder)
        {
            var chainedConfigurationSource = configurationBuilder.Sources.OfType<ChainedConfigurationSource>().First();
            configurationBuilder.Sources.Clear();
            configurationBuilder
                .Add(chainedConfigurationSource);
        }

        private static void AddConfigurationFromEnvironment(this IConfigurationBuilder configurationManager, string[] commandLineArgs)
        {
            configurationManager
                .AddJsonFile($"{Path.Combine(Program.CONFIG_FOLDER, Program.CONFIG_BASE_NAME)}.json", false, true)
                    .AddJsonFile($"{Path.Combine(Program.CONFIG_FOLDER, Program.CONFIG_BASE_NAME)}.Development.json", true, true)
                    .AddEnvironmentVariables()
                    .AddCommandLine(commandLineArgs);
        }

        private static void ConfigureNlog(this WebApplicationBuilder builder)
        {
            builder.Logging
                .ClearProviders()
               .SetMinimumLevel(LogLevel.Trace)
               .AddNLog(Path.Combine(Program.CONFIG_FOLDER, Program.NLOG_CONFIGURATION_FILE));

            builder.Host.UseNLog();
        }

        private static void ConfigureServerServices(WebHostBuilderContext context, IServiceCollection services)
        {
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

        private static void ConfigureServerApplication(this WebApplication webApplication)
        {
            if (webApplication.Environment.IsDevelopment())
            {
#if DEBUG
                webApplication.UseDeveloperExceptionPage();
                webApplication
                    .UseCors(policy => policy
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                    );
#endif
            }
            else
            {
                webApplication.UseHttpsRedirection();
            }

            webApplication.UseRouting();
            //webApplication.UseAuthorization();
            webApplication.RegisterOpenApiRoutes()
                .RegisterRestApiRoutes()
                .RegisterSpaRoutes()
                .MigrateDatabase();
        }

        private static IServiceCollection CreateAndRegisterEnvironmentConfiguration(this IServiceCollection services, IHostEnvironment hostEnvironment)
            => services.AddSingleton(new EnvironmentConfiguration
            {
                IsDevelopment = hostEnvironment.IsDevelopment(),
                IsProduction = hostEnvironment.IsProduction(),
            });

        private static void RegisterSpaStaticFiles(this IServiceCollection services, IHostEnvironment hostEnvironment)
        {
            //if (hostEnvironment.IsProduction())
            //    services.AddSpaStaticFiles(configuration => configuration.RootPath = Path.Combine(_executablePath, WEB_UI_FOLDER));

            services
                .AddSpaStaticFiles(configuration =>
                    configuration.RootPath = hostEnvironment.IsProduction()
                        ? Path.Combine(_executablePath, Program.WEB_UI_FOLDER)
                        : "../FS.TimeTracking.UI.Angular/dist/TimeTracking"
                );
        }

        [SuppressMessage("ReSharper", "UnusedParameter.Local", Justification = "Required, see commented code")]
        private static WebApplication RegisterSpaRoutes(this WebApplication webApplication)
        {
            //if (webApplication.Environment.IsProduction())
            //{
            //    webApplication.UseSpaStaticFiles();
            //    webApplication.UseSpa(_ => { });
            //}
            //else
            //{
            //    webApplication.UseSpa(c => c.UseProxyToSpaDevelopmentServer("http://localhost:4200/"));
            //}

            webApplication.UseSpaStaticFiles();
            webApplication.UseSpa(_ => { });

            return webApplication;
        }
    }
}
