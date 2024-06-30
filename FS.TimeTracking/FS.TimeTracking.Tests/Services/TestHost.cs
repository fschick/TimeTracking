using AutoMapper;
using FS.TimeTracking.Abstractions.Constants;
using FS.TimeTracking.Abstractions.DTOs.Administration;
using FS.TimeTracking.Api.REST.Extensions;
using FS.TimeTracking.Api.REST.Models;
using FS.TimeTracking.Application.Tests.Extensions;
using FS.TimeTracking.Application.Tests.Services;
using FS.TimeTracking.Application.Tests.Services.FakeServices;
using FS.TimeTracking.Core.Exceptions;
using FS.TimeTracking.Core.Models.Application.Core;
using FS.TimeTracking.Core.Models.Configuration;
using FS.TimeTracking.Tests.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace FS.TimeTracking.Tests.Services;

[ExcludeFromCodeCoverage]
public sealed class TestHost : IAsyncDisposable
{
    private readonly WebApplicationFactory<Program> _wepApplication;
    private readonly Faker _faker;
    private string _databaseFileToDeleteOnDispose;

    private TestHost(WebApplicationFactory<Program> wepApplication, Faker faker)
    {
        _wepApplication = wepApplication;
        _faker = faker;
    }

    public static async Task<TestHost> Create(IEnumerable<PermissionDto> permissionsForCurrentUser = null)
    {
        var databaseFile = $"IntegrationTest.{Guid.NewGuid()}.sqlite";
        var databaseConfiguration = new DatabaseConfiguration
        {
            Type = DatabaseType.InMemory,
            ConnectionString = $"Data Source={databaseFile};Pooling=false"
        };

        var result = await Create(databaseConfiguration, permissionsForCurrentUser);
        result._databaseFileToDeleteOnDispose = databaseFile;
        return result;
    }

    public static Task<TestHost> Create(DatabaseConfiguration databaseConfiguration, IEnumerable<PermissionDto> permissionsForCurrentUser = null)
    {
        var timeTrackingConfiguration = new TimeTrackingConfiguration { Database = databaseConfiguration, Features = new FeatureConfiguration { Reporting = true, Authorization = true } };
        var applicationConfiguration = new { TimeTracking = timeTrackingConfiguration };

        var faker = new Faker();

        var application = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(hostBuilder =>
            {
                //builder.UseTestServer();
                hostBuilder.ConfigureAppConfiguration((_, configurationBuilder) =>
                {
                    var configurationValues = applicationConfiguration.ToOptionsCollection();
                    configurationBuilder.AddInMemoryCollection(configurationValues);
                });

                hostBuilder.ConfigureServices((context, services) =>
                {
                    services.AddSingleton(Options.Create(timeTrackingConfiguration));
                    services.AddFeatureManagement(context.Configuration.GetSection("TimeTracking:Features"));
                });

                hostBuilder.ConfigureTestServices(services =>
                {
                    services.AddSingleton(faker.KeycloakRepository.Create());
                    permissionsForCurrentUser ??= DefaultPermissions.WritePermissions;
                    var currentUser = faker.User.Create("Current", permissions: permissionsForCurrentUser);
                    services
                        .Configure<TestAuthHandlerOptions>(options => options.CurrentUser = currentUser)
                        .AddAuthentication(TestAuthHandler.AUTHENTICATION_SCHEME)
                        .AddScheme<TestAuthHandlerOptions, TestAuthHandler>(TestAuthHandler.AUTHENTICATION_SCHEME, _ => { });
                });
            });

        var testHost = new TestHost(application, faker);
        return Task.FromResult(testHost);
    }

    public async Task<TResult> Get<TController, TResult>(Expression<Action<TController>> controllerAction)
    {
        using var client = GetTestClient();
        var route = GetRoute(controllerAction);
        using var response = await client.GetAsync(route);
        await EnsureSuccess(response);
        return await response.Content.ReadFromJsonAsync<TResult>();
    }

    public async Task<TResult> Get<TResult>(string route)
    {
        using var client = GetTestClient();
        using var response = await client.GetAsync(route);
        await EnsureSuccess(response);
        return await response.Content.ReadFromJsonAsync<TResult>();
    }

    public Task<TBody> Post<TController, TBody>(Expression<Action<TController>> controllerAction, TBody entity)
        => Post<TController, TBody, TBody>(controllerAction, entity);

    public async Task<TResult> Post<TController, TBody, TResult>(Expression<Action<TController>> controllerAction, TBody entity)
    {
        using var client = GetTestClient();
        var route = GetRoute(controllerAction);

        using var response = typeof(TBody).IsAssignableTo(typeof(IFormFile))
            ? await client.PostFormFile(route, (IFormFile)entity)
            : await client.PostAsJsonAsync(route, entity);

        await EnsureSuccess(response);
        if (response.StatusCode == HttpStatusCode.NoContent)
            return default;

        return await response.Content.ReadFromJsonAsync<TResult>();
    }

    public async Task<TBody> Put<TController, TBody>(Expression<Action<TController>> controllerAction, TBody entity)
    {
        using var client = GetTestClient();
        var route = GetRoute(controllerAction);
        using var response = await client.PutAsJsonAsync(route, entity);
        await EnsureSuccess(response);
        return await response.Content.ReadFromJsonAsync<TBody>();
    }

    public async Task Delete<TController>(Expression<Action<TController>> controllerAction)
    {
        using var client = GetTestClient();
        var route = GetRoute(controllerAction);
        using var response = await client.DeleteAsync(route);
        await EnsureSuccess(response);
    }

    private HttpClient GetTestClient()
        => _wepApplication.CreateClient();

    private string GetRoute<TController>(Expression<Action<TController>> controllerAction)
    {
        var apiDescriptionProvider = _wepApplication.Services.GetRequiredService<IApiDescriptionGroupCollectionProvider>();
        return ControllerExtensions.GetRoute(controllerAction, apiDescriptionProvider);
    }

    private static async Task EnsureSuccess(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
            return;

        switch (response.StatusCode)
        {
            case HttpStatusCode.Unauthorized:
                throw new UnauthorizedException();
            case HttpStatusCode.Forbidden:
                throw new ForbiddenException();
            case HttpStatusCode.BadRequest:
                throw new BadRequestException();
            case HttpStatusCode.Conflict:
                throw new ConflictException();
            default:
                var content = await response.Content.ReadAsStringAsync();
                var messages = JsonConvert.DeserializeObject<ApplicationError>(content).Messages.ToArray();
                throw new ApplicationErrorException(ApplicationErrorCode.InternalServerError, messages);
        }
    }

    #region IAsyncDisposable
    private bool _disposedValue;

    public async ValueTask DisposeAsync()
    {
        if (_disposedValue)
            return;

        await _wepApplication.DisposeAsync();
        _faker.Dispose();

        if (_databaseFileToDeleteOnDispose != null)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            File.Delete(_databaseFileToDeleteOnDispose);
        }

        _disposedValue = true;
    }
    #endregion

    public class TestAuthHandlerOptions : AuthenticationSchemeOptions
    {
        public UserDto CurrentUser { get; set; }
    }

    public class TestAuthHandler : AuthenticationHandler<TestAuthHandlerOptions>
    {
        public const string AUTHENTICATION_SCHEME = "IntegrationTest";

        private readonly ClaimsPrincipal _currentUserPrincipal;

        public TestAuthHandler(IOptionsMonitor<TestAuthHandlerOptions> options, ILoggerFactory logger, UrlEncoder encoder, IMapper mapper)
            : base(options, logger, encoder)
            => _currentUserPrincipal = FakeAuthorizationService.CreatePrincipal(options.CurrentValue.CurrentUser, mapper, AUTHENTICATION_SCHEME);

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var ticket = new AuthenticationTicket(_currentUserPrincipal, AUTHENTICATION_SCHEME);
            var result = AuthenticateResult.Success(ticket);
            return Task.FromResult(result);
        }
    }
}