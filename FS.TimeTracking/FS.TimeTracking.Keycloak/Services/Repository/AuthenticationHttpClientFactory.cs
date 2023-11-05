using FS.Keycloak.RestApiClient.Authentication.Client;
using FS.Keycloak.RestApiClient.Authentication.Flow;
using FS.TimeTracking.Core.Models.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace FS.TimeTracking.Keycloak.Services.Repository;

internal static class AuthenticationHttpClientFactory
{
    public static AuthenticationHttpClient Create(IServiceProvider serviceProvider)
    {
        var configuration = serviceProvider.GetRequiredService<IOptions<TimeTrackingConfiguration>>().Value.Keycloak;

        var passwordGrantFlow = new PasswordGrantFlow()
        {
            KeycloakUrl = configuration.AuthServerUrl,
            UserName = configuration.AdminUser,
            Password = configuration.AdminPassword,
        };

        return FS.Keycloak.RestApiClient.Authentication.ClientFactory.AuthenticationHttpClientFactory.Create(passwordGrantFlow);
    }
}