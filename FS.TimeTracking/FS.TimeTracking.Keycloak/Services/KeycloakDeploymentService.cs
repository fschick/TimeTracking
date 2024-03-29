﻿using FS.Keycloak.RestApiClient.Model;
using FS.TimeTracking.Abstractions.Constants;
using FS.TimeTracking.Core.Extensions;
using FS.TimeTracking.Core.Interfaces.Application.Services.Administration;
using FS.TimeTracking.Core.Interfaces.Application.Services.Shared;
using FS.TimeTracking.Core.Interfaces.Repository.Services.Administration;
using FS.TimeTracking.Core.Interfaces.Repository.Services.Database;
using FS.TimeTracking.Core.Models.Application.MasterData;
using FS.TimeTracking.Core.Models.Application.TimeTracking;
using FS.TimeTracking.Core.Models.Configuration;
using Microsoft.Extensions.Options;
using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Keycloak.Services;

/// <inheritdoc cref="IKeycloakDeploymentService" />
public sealed class KeycloakDeploymentService : IKeycloakDeploymentService
{
    private const string DEFAULT_USER_NAME = "admin";
    private const string DEFAULT_PASSWORD = "admin";

    private readonly TimeTrackingConfiguration _configuration;
    private readonly KeycloakConfiguration _keycloakConfiguration;
    private readonly IKeycloakRepository _keycloakRepository;
    private readonly IDbRepository _dbRepository;
    private readonly IInformationApiService _informationService;

    /// <summary>
    /// Initializes a new instance of the <see cref="KeycloakDeploymentService"/> class.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <param name="keycloakRepository">The keycloak repository.</param>
    /// <param name="dbRepository">The database repository.</param>
    /// <param name="informationService">The information service.</param>
    /// <autogeneratedoc />
    public KeycloakDeploymentService(IOptions<TimeTrackingConfiguration> configuration, IKeycloakRepository keycloakRepository, IDbRepository dbRepository, IInformationApiService informationService)
    {
        _configuration = configuration.Value;
        _keycloakConfiguration = _configuration.Keycloak;
        _keycloakRepository = keycloakRepository;
        _dbRepository = dbRepository;
        _informationService = informationService;
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

        var productName = await _informationService.GetProductName(cancellationToken);
        var realm = await CreateRealm(productName, cancellationToken);
        var clientId = await CreateClient(realm, productName, cancellationToken);
        var audienceScopeId = await CreateAudienceClientScope(realm, cancellationToken);
        await _keycloakRepository.AddScopeToClient(realm, clientId, audienceScopeId, cancellationToken);
        var restrictToCustomerScopeId = await CreateRestrictToCustomerIdClientScope(realm, cancellationToken);
        await _keycloakRepository.AddScopeToClient(realm, clientId, restrictToCustomerScopeId, cancellationToken);
        var adminUserId = await CreateDefaultAdminUser(realm, cancellationToken);
        var clientRoles = await CreateClientRoles(realm, clientId, cancellationToken);
        await _keycloakRepository.AddClientRolesToUser(realm, adminUserId, clientId, clientRoles, cancellationToken);

        return true;
    }

    /// <inheritdoc />
    public async Task SyncClientRoles(CancellationToken cancellationToken = default)
    {
        if (!_configuration.Features.Authorization)
            return;

        var realm = _keycloakConfiguration.Realm;
        var clientId = _keycloakConfiguration.ClientId;

        var realms = await _keycloakRepository
            .GetRealms(cancellationToken)
            .FirstOrDefaultAsync(x => x.Realm == realm)
            ?? throw new InvalidOperationException($"Keycloak realm '{realm}' not found");

        var client = await _keycloakRepository
            .GetClients(realms.Realm, cancellationToken)
            .FirstOrDefaultAsync(x => x.ClientId == clientId)
            ?? throw new InvalidOperationException($"Keycloak client '{clientId}' not found");

        var savedRoles = await _keycloakRepository.GetClientRoles(realm, client.Id, cancellationToken);
        var currentRoles = CreateAllRoles(clientId);

        var rolesToAdd = currentRoles.ExceptBy(savedRoles, x => x.Name).ToList();
        var rolesToRemove = savedRoles.ExceptBy(currentRoles, x => x.Name).ToList();

        await rolesToAdd
            .Select(async role => await _keycloakRepository.CreateClientRole(realm, client.Id, role, cancellationToken))
            .WhenAll();

        await rolesToRemove
            .Select(async role => await _keycloakRepository.DeleteClientRole(realm, client.Id, role, cancellationToken))
            .WhenAll();
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

    private async Task<string> CreateRealm(string productName, CancellationToken cancellationToken = default)
    {
        var realm = new RealmRepresentation
        {
            //Id = Guid.NewGuid().ToString(),
            Realm = _keycloakConfiguration.Realm,
            DisplayName = productName,
            Enabled = true,
            EditUsernameAllowed = true,
            RememberMe = true,
            SsoSessionIdleTimeoutRememberMe = 7 * 24 * 60 * 60,
            SsoSessionMaxLifespanRememberMe = 30 * 24 * 60 * 60,
        };

        await _keycloakRepository.CreateRealm(realm, cancellationToken);

        return realm.Realm;
    }

    private async Task<string> CreateClient(string realm, string productName, CancellationToken cancellationToken)
    {
        var client = new ClientRepresentation
        {
            ClientId = _keycloakConfiguration.ClientId,
            Name = productName,
            ClientAuthenticatorType = "",
            PublicClient = true,
            FrontchannelLogout = true,
            DirectAccessGrantsEnabled = false,
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
                    { "included.client.audience", _keycloakConfiguration.ClientId},
                    { "access.token.claim", "true"},
                }
            } }
        };

        await _keycloakRepository.CreateClientScope(realm, clientScope, cancellationToken);

        var createdClientScope = await _keycloakRepository.GetClientScopes(realm, cancellationToken)
            .FirstAsync(storedScope => storedScope.Name == audienceName);

        return createdClientScope.Id;
    }

    private async Task<string> CreateRestrictToCustomerIdClientScope(string realm, CancellationToken cancellationToken)
    {
        var clientScope = new ClientScopeRepresentation
        {
            Name = RestrictToCustomer.CLIENT_SCOPE,
            Protocol = "openid-connect",
            ProtocolMappers = new List<ProtocolMapperRepresentation> { new() {
                Id = Guid.NewGuid().ToString(),
                Name = RestrictToCustomer.MAPPER,
                Protocol = "openid-connect",
                ProtocolMapper = "oidc-usermodel-attribute-mapper",
                ConsentRequired = false,
                Config = new Dictionary<string, string>{
                    { "multivalued", "true"},
                    { "access.token.claim", "true"},
                    { "id.token.claim", "false"},
                    { "userinfo.token.claim", "false"},
                    { "claim.name", RestrictToCustomer.CLAIM},
                    { "user.attribute", RestrictToCustomer.ATTRIBUTE},
                }
            } }
        };

        await _keycloakRepository.CreateClientScope(realm, clientScope, cancellationToken);

        var createdClientScope = await _keycloakRepository.GetClientScopes(realm, cancellationToken)
            .FirstAsync(storedScope => storedScope.Name == RestrictToCustomer.CLIENT_SCOPE);

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
                    Value = DEFAULT_PASSWORD,
                    Temporary = true,
                },
            },
        };

        await _keycloakRepository.CreateUser(realm, user, cancellationToken);

        var createdUser = await _keycloakRepository
            .GetUsers(realm, username: user.Username, cancellationToken: cancellationToken)
            .FirstAsync();

        return Guid.Parse(createdUser.Id);
    }

    private async Task<List<RoleRepresentation>> CreateClientRoles(string realm, string clientId, CancellationToken cancellationToken)
    {
        var roles = CreateAllRoles(clientId);

        await roles.Select(async role => await _keycloakRepository.CreateClientRole(realm, clientId, role, cancellationToken)).WhenAll();

        var storedRoles = await _keycloakRepository.GetClientRoles(realm, clientId, cancellationToken)
            .WhereAsync(storedRole => roles.Any(role => role.Name == storedRole.Name))
            .ToListAsync();

        return storedRoles;
    }

    private static List<RoleRepresentation> CreateAllRoles(string clientId)
        => RoleName.All
            .Select(role => new RoleRepresentation
            {
                Name = role,
                ContainerId = clientId,
                ClientRole = true,
            })
            .ToList();
}