using FS.TimeTracking.Api.REST.Startup;
using FS.TimeTracking.Application.Startup;
using FS.TimeTracking.Shared.Extensions;
using FS.TimeTracking.Shared.Models.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

namespace FS.TimeTracking.Startup;

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
            .AddJsonFile($"{Path.Combine(Program.CONFIG_FOLDER, Program.CONFIG_BASE_NAME)}.json", false, true)
            .AddJsonFile($"{Path.Combine(Program.CONFIG_FOLDER, Program.CONFIG_BASE_NAME)}.Development.json", true, true)
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
                .Configure<TimeTrackingConfiguration>(context.Configuration.GetSection(TimeTrackingConfiguration.CONFIGURATION_SECTION));
        });
    }

    private static void ConfigureNlog(this WebApplicationBuilder builder)
    {
        builder.Logging
            .ClearProviders()
            .SetMinimumLevel(LogLevel.Trace)
            .AddNLog(Path.Combine(Program.CONFIG_FOLDER, Program.NLOG_CONFIGURATION_FILE));

        builder.Host.UseNLog();
    }

    private static void ConfigureServerServices(this WebApplicationBuilder builder)
    {
        builder.WebHost.ConfigureServices((context, services) =>
        {
            services
                .RegisterTimeTrackingAutoMapper()
                .RegisterFilterExpressionInterceptor()
                .RegisterApplicationServices()
                .RegisterOpenApiController()
                .RegisterRestApiController()
                .RegisterSpaStaticFiles(context.HostingEnvironment);
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
            .RegisterRestApiRoutes()
            .RegisterSpaRoutes();
    }

    private static IServiceCollection CreateAndRegisterEnvironmentConfiguration(this IServiceCollection services, IHostEnvironment hostEnvironment)
        => services.AddSingleton(new EnvironmentConfiguration
        {
            IsDevelopment = hostEnvironment.IsDevelopment(),
            IsProduction = hostEnvironment.IsProduction(),
        });

    private static void RegisterSpaStaticFiles(this IServiceCollection services, IHostEnvironment hostEnvironment)
        => services.AddSpaStaticFiles(configuration => configuration.RootPath = GetWebRootPath(hostEnvironment));

    private static string GetWebRootPath(IHostEnvironment hostEnvironment)
        => hostEnvironment.IsProduction()
            ? Path.Combine(_executablePath, Program.WEB_UI_FOLDER)
            : "../../FS.TimeTracking.UI.Angular/dist/TimeTracking".Replace('/', Path.DirectorySeparatorChar);

    [SuppressMessage("ReSharper", "UnusedParameter.Local", Justification = "Required, see commented code")]
    private static void RegisterSpaRoutes(this WebApplication webApplication)
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

        //return webApplication;
    }
}