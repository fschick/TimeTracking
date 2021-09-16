using FS.TimeTracking.Shared.Models.Configuration;
using FS.TimeTracking.Tests.Extensions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Tests.Services
{
    public sealed class TestHost : IHost, IAsyncDisposable
    {
        private readonly IHost _testServer;

        private TestHost(IHost testServer)
            => _testServer = testServer;

        public static async Task<TestHost> Create(DatabaseConfiguration databaseConfiguration)
        {
            var configuration = new TimeTrackingConfiguration { Database = databaseConfiguration };

            var hostBuilder = Program.CreateHostBuilderInternal(configuration)
                .ConfigureWebHost(webHostBuilder => webHostBuilder.UseTestServer());

            var testServer = await hostBuilder.StartAsync();
            return new TestHost(testServer);
        }

        public string GetRoute<TController>(Expression<Action<TController>> controllerAction)
        {
            var apiDescriptionProvider = _testServer.Services.GetRequiredService<IApiDescriptionGroupCollectionProvider>();
            return ControllerExtensions.GetRoute(controllerAction, apiDescriptionProvider);
        }

        #region IHost
        public IServiceProvider Services => _testServer.Services;

        public async Task StartAsync(CancellationToken cancellationToken = default)
            => await _testServer.StartAsync(cancellationToken);

        public async Task StopAsync(CancellationToken cancellationToken = default)
            => await _testServer.StopAsync(cancellationToken);

        public void Dispose()
            => Task.WaitAll(DisposeAsync().AsTask());
        #endregion

        #region IAsyncDisposable
        private bool _disposedValue;

        public async ValueTask DisposeAsync()
        {
            if (_disposedValue)
                return;

            using var stopServerCancellation = new CancellationTokenSource();
            stopServerCancellation.CancelAfter(TimeSpan.FromSeconds(10));
            await StopAsync(stopServerCancellation.Token);
            _testServer.Dispose();

            _disposedValue = true;
        }
        #endregion
    }
}
