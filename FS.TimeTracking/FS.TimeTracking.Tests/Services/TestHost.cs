using FS.TimeTracking.Api.REST.Extensions;
using FS.TimeTracking.Core.Models.Configuration;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;
using Newtonsoft.Json;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq.Expressions;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace FS.TimeTracking.Tests.Services;

[ExcludeFromCodeCoverage]
public sealed class TestHost : IAsyncDisposable
{
    private readonly WebApplicationFactory<Program> _wepApplication;

    private TestHost(WebApplicationFactory<Program> wepApplication)
        => _wepApplication = wepApplication;

    public static Task<TestHost> Create(DatabaseConfiguration databaseConfiguration)
    {
        var timeTrackingConfiguration = new TimeTrackingConfiguration { Database = databaseConfiguration, Features = new FeatureConfiguration { Reporting = true } };
        var applicationConfiguration = new { TimeTracking = timeTrackingConfiguration };

        var application = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(hostBuilder =>
            {
                //builder.UseTestServer();
                hostBuilder.ConfigureAppConfiguration((_, configurationBuilder) =>
                {
                    var appSettingsJson = JsonConvert.SerializeObject(applicationConfiguration);
#pragma warning disable IDISP001 // Dispose created
                    // Justification: No workable way found found to dispose, stream must be readable by TestHost later
                    var appSettingsStream = new MemoryStream(Encoding.UTF8.GetBytes(appSettingsJson));
#pragma warning restore IDISP001 // Dispose created

                    configurationBuilder.AddJsonStream(appSettingsStream);
                });
                hostBuilder.ConfigureServices((context, services) =>
                {
                    services.AddSingleton(Options.Create(timeTrackingConfiguration));
                    services.AddFeatureManagement(context.Configuration.GetSection("TimeTracking:Features"));
                });
            });

        var testHost = new TestHost(application);
        return Task.FromResult(testHost);
    }

    public HttpClient GetTestClient()
        => _wepApplication.CreateClient();

    public string GetRoute<TController>(Expression<Action<TController>> controllerAction)
    {
        var apiDescriptionProvider = _wepApplication.Services.GetRequiredService<IApiDescriptionGroupCollectionProvider>();
        return ControllerExtensions.GetRoute(controllerAction, apiDescriptionProvider);
    }

    public async Task<TEntity> Get<TController, TEntity>(Expression<Action<TController>> controllerAction)
    {
        using var client = GetTestClient();
        var route = GetRoute(controllerAction);
        return await client.GetFromJsonAsync<TEntity>(route);
    }

    public async Task<TEntity> Get<TEntity>(string route)
    {
        using var client = GetTestClient();
        return await client.GetFromJsonAsync<TEntity>(route);
    }

    public async Task<TEntity> Post<TController, TEntity>(Expression<Action<TController>> controllerAction, TEntity entity)
    {
        using var client = GetTestClient();
        var route = GetRoute(controllerAction);
        using var response = await client.PostAsJsonAsync(route, entity);
        return await response.Content.ReadFromJsonAsync<TEntity>();
    }

    public async Task Delete<TController>(Expression<Action<TController>> controllerAction)
    {
        using var client = GetTestClient();
        var route = GetRoute(controllerAction);
        await client.DeleteAsync(route);
    }

    #region IAsyncDisposable
    private bool _disposedValue;

    public async ValueTask DisposeAsync()
    {
        if (_disposedValue)
            return;

        await _wepApplication.DisposeAsync();

        _disposedValue = true;
    }
    #endregion
}