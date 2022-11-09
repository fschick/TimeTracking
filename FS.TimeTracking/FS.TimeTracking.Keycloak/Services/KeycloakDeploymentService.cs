﻿using FS.Keycloak.RestApiClient.Model;
using FS.TimeTracking.Abstractions.Constants;
using FS.TimeTracking.Core.Extensions;
using FS.TimeTracking.Core.Interfaces.Application.Services.Administration;
using FS.TimeTracking.Core.Interfaces.Repository.Services.Administration;
using FS.TimeTracking.Core.Interfaces.Repository.Services.Database;
using FS.TimeTracking.Core.Models.Application.MasterData;
using FS.TimeTracking.Core.Models.Application.TimeTracking;
using FS.TimeTracking.Core.Models.Configuration;
using Microsoft.Extensions.Options;
using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Keycloak.Services;

/// <inheritdoc cref="IKeycloakDeploymentService" />
public sealed class KeycloakDeploymentService : IKeycloakDeploymentService
{
    private const string DEFAULT_USER_NAME = "admin";

    private readonly TimeTrackingConfiguration _configuration;
    private readonly KeycloakConfiguration _keycloakConfiguration;
    private readonly IKeycloakRepository _keycloakRepository;
    private readonly IDbRepository _dbRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="KeycloakDeploymentService"/> class.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <param name="keycloakRepository">The keycloak repository.</param>
    /// <param name="dbRepository">The database repository.</param>
    /// <autogeneratedoc />
    public KeycloakDeploymentService(IOptions<TimeTrackingConfiguration> configuration, IKeycloakRepository keycloakRepository, IDbRepository dbRepository)
    {
        _configuration = configuration.Value;
        _keycloakConfiguration = _configuration.Keycloak;
        _keycloakRepository = keycloakRepository;
        _dbRepository = dbRepository;
    }

    /// <inheritdoc />
    public async Task<bool> CreateRealmIfNotExists(CancellationToken cancellationToken = default)
    {
        if (!_configuration.Features.Authorization || !_configuration.Keycloak.CreateRealm)
            return false;

        var realms = await _keycloakRepository.GetRealms(cancellationToken);
        var realmAlreadyExists = realms.Any(x => x.Realm == _keycloakConfiguration.Realm);
        if (realmAlreadyExists)
            return false;

        var realm = await CreateRealm(cancellationToken);
        var clientId = await CreateClient(realm, cancellationToken);
        var audienceId = await CreateAudienceClientScope(realm, cancellationToken);
        await _keycloakRepository.AddScopeToClient(realm, clientId, audienceId, cancellationToken);
        var adminUserId = await CreateDefaultAdminUser(realm, cancellationToken);
        var clientRoles = await CreateClientRoles(realm, clientId, cancellationToken);
        await _keycloakRepository.AddClientRolesToUser(realm, adminUserId, clientId, clientRoles, cancellationToken);

        return true;
    }

    /// <inheritdoc />
    public async Task DeleteRealmIfExists(CancellationToken cancellationToken = default)
        => await _keycloakRepository.DeleteRealm(_keycloakConfiguration.Realm, cancellationToken);

    /// <inheritdoc />
    public async Task SetUserIdOfRelatedEntitiesToDefaultUser(CancellationToken cancellationToken = default)
    {
        var defaultUser = await _keycloakRepository
            .GetUsers(_keycloakConfiguration.Realm, username: DEFAULT_USER_NAME, cancellationToken: cancellationToken)
            .FirstOrDefaultAsync();
        var defaultUserId = new Guid(defaultUser.Id);
        await _dbRepository.BulkUpdate((TimeSheet t) => t.UserId == Guid.Empty, _ => new TimeSheet { UserId = defaultUserId });
        await _dbRepository.BulkUpdate((Holiday t) => t.UserId == Guid.Empty, _ => new Holiday { UserId = defaultUserId });
    }

    private async Task<string> CreateRealm(CancellationToken cancellationToken = default)
    {
        var realm = new RealmRepresentation
        {
            //Id = Guid.NewGuid().ToString(),
            Realm = _keycloakConfiguration.Realm,
            Enabled = true,
            EditUsernameAllowed = true,
        };

        await _keycloakRepository.CreateRealm(realm, cancellationToken);

        return realm.Realm;
    }

    private async Task<string> CreateClient(string realm, CancellationToken cancellationToken)
    {
        var client = new ClientRepresentation
        {
            ClientId = _keycloakConfiguration.ClientId,
            Name = _keycloakConfiguration.ClientId,
            ClientAuthenticatorType = "",
            PublicClient = true,
            FrontchannelLogout = true,
            DirectAccessGrantsEnabled = true,
            RedirectUris = new List<string> { "*" },
            WebOrigins = new List<string> { "*" },
            Attributes = new Dictionary<string, string> { { "post.logout.redirect.uris", "*" } },
        };

        await _keycloakRepository.CreateClient(realm, client, cancellationToken);

        var createdClient = await _keycloakRepository
            .GetClients(realm, cancellationToken)
            .FirstAsync(storedClient => storedClient.ClientId == _keycloakConfiguration.ClientId);

        return createdClient.Id;
    }

    private async Task<string> CreateAudienceClientScope(string realm, CancellationToken cancellationToken)
    {
        // Configure audience in Keycloak, https://stackoverflow.com/a/53627747
        var audienceName = $"{_keycloakConfiguration.ClientId}-audience";
        var clientScope = new ClientScopeRepresentation
        {
            Name = audienceName,
            Protocol = "openid-connect",
            ProtocolMappers = new List<ProtocolMapperRepresentation> { new() {
                Id = Guid.NewGuid().ToString(),
                Name = audienceName,
                Protocol = "openid-connect",
                ProtocolMapper = "oidc-audience-mapper",
                Config = new Dictionary<string, string>{
                    { "included.client.audience", _keycloakConfiguration.ClientId} ,
                    { "access.token.claim", "true"} ,
                }
            } }
        };

        await _keycloakRepository.CreateClientScope(realm, clientScope, cancellationToken);

        var createdClientScope = await _keycloakRepository.GetClientScopes(realm, cancellationToken)
            .FirstAsync(storedScope => storedScope.Name == audienceName);

        return createdClientScope.Id;
    }

    private async Task<Guid> CreateDefaultAdminUser(string realm, CancellationToken cancellationToken)
    {
        var user = new UserRepresentation
        {
            Username = DEFAULT_USER_NAME,
            Enabled = true,
            Credentials = new List<CredentialRepresentation>
            {
                new() {
                    Type = "password",
                    Value = DEFAULT_USER_NAME,
                    Temporary = true,
                },
            },
        };

        await _keycloakRepository.CreateUser(realm, user, cancellationToken);

        var createdUser = await _keycloakRepository.GetUsers(realm, username: user.Username, cancellationToken: cancellationToken)
            .FirstAsync();

        return Guid.Parse(createdUser.Id);
    }

    private async Task<List<RoleRepresentation>> CreateClientRoles(string realm, string clientId, CancellationToken cancellationToken)
    {
        var roles = RoleNames.All
            .Select(role => new RoleRepresentation
            {
                Name = role,
                ContainerId = clientId,
                ClientRole = true,
            })
            .ToList();

        await roles.Select(async role => await _keycloakRepository.CreateClientRole(realm, clientId, role, cancellationToken)).WhenAll();

        var storedRoles = await _keycloakRepository.GetClientRoles(realm, clientId, cancellationToken)
            .WhereAsync(storedRole => roles.Any(role => role.Name == storedRole.Name))
            .ToListAsync();

        return storedRoles;
    }
}