using FS.Authentication.OneTimeToken.Extensions;
using FS.TimeTracking.Api.REST.Startup;
using FS.TimeTracking.Application.Startup;
using FS.TimeTracking.Core.Models.Configuration;
using FS.TimeTracking.Keycloak.Startup;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;
using NLog.Web;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

namespace FS.TimeTracking.Startup;

internal static class TimeTrackingWebApp
{
    public static WebApplication Create(string[] args)
    {
        var webAppBuilder = CreateWebApplicationBuilder(args);
        var webApp = webAppBuilder.Build();
        webApp.ConfigureServerApplication();
        return webApp;
    }

    public static WebApplicationBuilder CreateWebApplicationBuilder(string[] args)
    {
        var options = new WebApplicationOptions { Args = args, ContentRootPath = TimeTrackingConfiguration.PathToContentRoot };
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
            .AddJsonFile($"{Path.Combine(TimeTrackingConfiguration.CONFIG_FOLDER, TimeTrackingConfiguration.CONFIG_BASE_NAME)}.json", false, true)
            .AddJsonFile($"{Path.Combine(TimeTrackingConfiguration.CONFIG_FOLDER, TimeTrackingConfiguration.CONFIG_BASE_NAME)}.Development.json", true, true)
            .AddEnvironmentVariables()
            .AddCommandLine(commandLineArgs);
    }

    private static void CreateServerConfiguration(this WebApplicationBuilder builder, string[] args)
    {
        builder.Configuration.ClearConfiguration();
        builder.Configuration.AddConfigurationFromEnvironment(args);
        builder.Services.CreateAndRegisterEnvironmentConfiguration(builder.Environment);
        builder.Services.CreateAndRegisterTimeTrackingConfiguration(builder.Configuration);
    }

    private static void ConfigureNlog(this WebApplicationBuilder builder)
    {
        builder.Logging
            .ClearProviders()
            .SetMinimumLevel(LogLevel.Trace)
            .AddNLog(Path.Combine(TimeTrackingConfiguration.CONFIG_FOLDER, TimeTrackingConfiguration.NLOG_CONFIGURATION_FILE));

        builder.Host.UseNLog();
    }

    private static void ConfigureServerServices(this WebApplicationBuilder builder)
    {
        var configuration = builder.Configuration.ReadTimeTrackingConfiguration();

        builder.Services.AddFeatureManagement(builder.Configuration.GetSection("TimeTracking:Features"));
        builder.Services.RegisterTimeTrackingAutoMapper();
        builder.Services.RegisterFilterExpressionInterceptor();
        builder.Services.RegisterApplicationServices();
        builder.Services.RegisterOpenApiController(configuration);
        builder.Services.RegisterRestApiController();
        builder.Services.RegisterAuthorizationPolicies();
        builder.Services.RegisterSpaStaticFiles(builder.Environment);
        builder.Services.RegisterKeycloakAuthentication(configuration);
        builder.Services.AddOneTimeTokenAuthentication(o => o.NameIdentifier = Guid.Empty.ToString());
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
        webApplication.UseAuthentication();
        webApplication.UseAuthorization();
        webApplication
            .RegisterOpenApiRoutes()
            .RegisterOpenApiUiRedirects()
            .RegisterRestApiRoutes()
            .RegisterSpaRoutes();
    }

    private static IServiceCollection CreateAndRegisterEnvironmentConfiguration(this IServiceCollection services, IHostEnvironment hostEnvironment)
        => services.AddSingleton(new EnvironmentConfiguration
        {
            IsDevelopment = hostEnvironment.IsDevelopment(),
            IsProduction = hostEnvironment.IsProduction(),
        });

    private static void CreateAndRegisterTimeTrackingConfiguration(this IServiceCollection services, IConfiguration configuration)
        => services.Configure<TimeTrackingConfiguration>(configuration.GetSection(TimeTrackingConfiguration.CONFIGURATION_SECTION));

    public static TimeTrackingConfiguration ReadTimeTrackingConfiguration(this IConfiguration configuration)
    {
        var timeTrackingConfigurationSection = configuration.GetSection(TimeTrackingConfiguration.CONFIGURATION_SECTION);
        var timeTrackingConfiguration = new TimeTrackingConfiguration();
        timeTrackingConfigurationSection.Bind(timeTrackingConfiguration);
        return timeTrackingConfiguration;
    }

    private static void RegisterSpaStaticFiles(this IServiceCollection services, IHostEnvironment hostEnvironment)
        => services.AddSpaStaticFiles(configuration => configuration.RootPath = GetWebRootPath(hostEnvironment));

    private static string GetWebRootPath(IHostEnvironment hostEnvironment)
        => hostEnvironment.IsProduction()
            ? Path.Combine(TimeTrackingConfiguration.ExecutablePath, TimeTrackingConfiguration.WEB_UI_FOLDER)
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