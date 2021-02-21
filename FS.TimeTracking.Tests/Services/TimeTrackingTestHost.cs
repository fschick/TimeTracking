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
    public sealed class TimeTrackingTestHost : IHost, IAsyncDisposable
    {
        private IHost _testHost;

        private TimeTrackingTestHost() { }

        public static async Task<TimeTrackingTestHost> Create(DatabaseConfiguration database)
        {
            var configuration = new TimeTrackingConfiguration { Database = database };

            var hostBuilder = Program.CreateHostBuilderInternal(configuration)
                .ConfigureWebHost(webHostBuilder => webHostBuilder.UseTestServer());

            return new TimeTrackingTestHost
            {
                _testHost = await hostBuilder.StartAsync(),
            };
        }

        public string GetRoute<TController>(Expression<Action<TController>> controllerAction)
        {
            var apiDescriptionProvider = _testHost.Services.GetRequiredService<IApiDescriptionGroupCollectionProvider>();
            return ControllerExtensions.GetRoute(controllerAction, apiDescriptionProvider);
        }

        #region IHost
        public IServiceProvider Services => _testHost.Services;

        public Task StartAsync(CancellationToken cancellationToken = default)
            => throw new InvalidOperationException($"{nameof(TimeTrackingTestHost)} cannot be started directly. Use ${nameof(TimeTrackingTestHost)}.{nameof(Create)} instead.");

        public Task StopAsync(CancellationToken cancellationToken = default)
            => throw new InvalidOperationException($"{nameof(TimeTrackingTestHost)} cannot be stopped directly. Dispose the instance of ${nameof(TimeTrackingTestHost)} created by ${nameof(TimeTrackingTestHost)}.{nameof(Create)} instead.");

        public void Dispose()
            => Task.WaitAll(DisposeAsync().AsTask());
        #endregion

        #region IAsyncDisposable
        private bool _disposedValue;

        public async ValueTask DisposeAsync()
        {
            if (_disposedValue)
                return;

            await _testHost.StopAsync(TimeSpan.FromSeconds(10));
            _testHost.Dispose();

            _disposedValue = true;
        }
        #endregion
    }
}
