using FS.Keycloak.RestApiClient.Client;
using FS.TimeTracking.Core.Models.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace FS.TimeTracking.Repository.Services.Administration;

internal static class KeycloakHttpClientFactory
{
    public static KeycloakHttpClient Create(IServiceProvider serviceProvider)
    {
        var configuration = serviceProvider.GetRequiredService<IOptions<TimeTrackingConfiguration>>().Value.Keycloak;
        return new KeycloakHttpClient(configuration.AuthServerUrl, configuration.AdminUser, configuration.AdminPassword);
    }
}