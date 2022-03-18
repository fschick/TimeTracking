using FS.TimeTracking.Report.Abstractions.Models.Configuration;
using FS.TimeTracking.Report.Api.REST.Startup;
using FS.TimeTracking.Shared.Models.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;
using System.IO;
using System.Linq;

namespace FS.TimeTracking.Report.Startup;

internal static class TimeTrackingWebApp
{


    internal static WebApplication Create(string[] args)
    {
        var webAppBuilder = CreateWebApplicationBuilder(args);
        var webApp = webAppBuilder.Build();
        webApp.ConfigureServerApplication();
        return webApp;
    }

    internal static WebApplicationBuilder CreateWebApplicationBuilder(string[] args)
    {
        var options = new WebApplicationOptions { Args = args, ContentRootPath = TimeTrackingReportConfiguration.PathToContentRoot };
        var webAppBuilder = WebApplication.CreateBuilder(options);

        webAppBuilder.CreateServerConfiguration(args);
        webAppBuilder.ConfigureNlog();
        webAppBuilder.ConfigureServerServices();

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
            .AddJsonFile($"{Path.Combine(TimeTrackingReportConfiguration.CONFIG_FOLDER, TimeTrackingReportConfiguration.CONFIG_BASE_NAME)}.json", false, true)
            .AddJsonFile($"{Path.Combine(TimeTrackingReportConfiguration.CONFIG_FOLDER, TimeTrackingReportConfiguration.CONFIG_BASE_NAME)}.Development.json", true, true)
            .AddEnvironmentVariables()
            .AddCommandLine(commandLineArgs);
    }

    private static void CreateServerConfiguration(this WebApplicationBuilder builder, string[] args)
    {
        builder.Configuration.ClearConfiguration();
        builder.Configuration.AddConfigurationFromEnvironment(args);

        builder.WebHost.ConfigureServices((context, services) =>
        {
            services
                .CreateAndRegisterEnvironmentConfiguration(context.HostingEnvironment)
                .Configure<TimeTrackingReportConfiguration>(context.Configuration.GetSection(TimeTrackingReportConfiguration.CONFIGURATION_SECTION));
        });
    }

    private static void ConfigureNlog(this WebApplicationBuilder builder)
    {
        builder.Logging
            .ClearProviders()
            .SetMinimumLevel(LogLevel.Trace)
            .AddNLog(Path.Combine(TimeTrackingReportConfiguration.CONFIG_FOLDER, TimeTrackingReportConfiguration.NLOG_CONFIGURATION_FILE));

        builder.Host.UseNLog();
    }

    private static void ConfigureServerServices(this WebApplicationBuilder builder)
    {
        builder.WebHost.ConfigureServices((context, services) =>
        {
            services
                .RegisterApplicationServices()
                .RegisterOpenApiController()
                .RegisterRestApiController();
        });
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
        webApplication
            .RegisterOpenApiRoutes()
            .RegisterRestApiRoutes();
    }

    private static IServiceCollection CreateAndRegisterEnvironmentConfiguration(this IServiceCollection services, IHostEnvironment hostEnvironment)
        => services.AddSingleton(new EnvironmentConfiguration
        {
            IsDevelopment = hostEnvironment.IsDevelopment(),
            IsProduction = hostEnvironment.IsProduction(),
        });
}