using FS.TimeTracking.Abstractions.Models.Configuration;
using FS.TimeTracking.Tests.Extensions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Net.Http;
using System.Net.Http.Json;
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
        var configuration = new TimeTrackingConfiguration { Database = databaseConfiguration };

        var application = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                //builder.UseTestServer();
                builder.ConfigureServices(services => services.AddSingleton(Options.Create(configuration)));
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
        var response = await client.PostAsJsonAsync(route, entity);
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