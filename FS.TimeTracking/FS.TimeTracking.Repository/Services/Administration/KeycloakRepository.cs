﻿using FS.Keycloak.RestApiClient.Api;
using FS.Keycloak.RestApiClient.Model;
using FS.TimeTracking.Core.Interfaces.Repository.Services.Administration;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Repository.Services.Administration;

/// <inheritdoc />
public class KeycloakRepository : IKeycloakRepository
{
    private readonly IRealmsAdminApi _realmsService;
    private readonly IRealmAdminApi _realmService;
    private readonly IClientsApi _clientsService;
    private readonly IClientApi _clientService;
    private readonly IClientScopesApi _clientScopesService;
    private readonly IRoleContainerApi _roleContainerApi;
    private readonly IUsersApi _usersService;
    private readonly IClientRoleMappingsApi _clientRoleMappingsApi;

    /// <summary>
    /// Initializes a new instance of the <see cref="KeycloakRepository"/> class.
    /// </summary>
    /// <param name="realmsService">The realms service.</param>
    /// <param name="realmService">The realm service.</param>
    /// <param name="clientsService">The clients service.</param>
    /// <param name="clientService">The client service.</param>
    /// <param name="clientScopesService">The client scopes service.</param>
    /// <param name="roleContainerApi">The role container API.</param>
    /// <param name="usersService">The users service.</param>
    /// <param name="clientRoleMappingsApi">The client role mappings API.</param>
    public KeycloakRepository(IRealmsAdminApi realmsService, IRealmAdminApi realmService, IClientsApi clientsService, IClientApi clientService, IClientScopesApi clientScopesService, IRoleContainerApi roleContainerApi, IUsersApi usersService, IClientRoleMappingsApi clientRoleMappingsApi)
    {
        _realmsService = realmsService;
        _realmService = realmService;
        _clientsService = clientsService;
        _clientService = clientService;
        _clientScopesService = clientScopesService;
        _roleContainerApi = roleContainerApi;
        _usersService = usersService;
        _clientRoleMappingsApi = clientRoleMappingsApi;
    }

    /// <inheritdoc />
    public async Task<List<RealmRepresentation>> GetRealms(CancellationToken cancellationToken = default)
        => await _realmsService.GetAsync(cancellationToken: cancellationToken);

    /// <inheritdoc />
    public async Task CreateRealm(RealmRepresentation realm, CancellationToken cancellationToken = default)
    => await _realmsService.PostAsync(realm, cancellationToken);

    /// <inheritdoc />
    public async Task DeleteRealm(string realm, CancellationToken cancellationToken = default)
        => await _realmService.DeleteAsync(realm, cancellationToken);

    /// <inheritdoc />
    public async Task<List<ClientRepresentation>> GetClients(string realm, CancellationToken cancellationToken = default)
        => await _clientsService.GetClientsAsync(realm, cancellationToken: cancellationToken);

    /// <inheritdoc />
    public async Task CreateClient(string realm, ClientRepresentation client, CancellationToken cancellationToken = default)
        => await _clientsService.PostClientsAsync(realm, client, cancellationToken);

    /// <inheritdoc />
    public async Task<List<ClientScopeRepresentation>> GetClientScopes(string realm, CancellationToken cancellationToken = default)
        => await _clientScopesService.GetClientScopesAsync(realm, cancellationToken);

    /// <inheritdoc />
    public async Task CreateClientScope(string realm, ClientScopeRepresentation clientScope, CancellationToken cancellationToken = default)
        => await _clientScopesService.PostClientScopesAsync(realm, clientScope, cancellationToken);

    /// <inheritdoc />
    public async Task AddScopeToClient(string realm, string clientId, string clientScopeId, CancellationToken cancellationToken)
        => await _clientService.PutClientsDefaultClientScopesByIdAndClientScopeIdAsync(realm, clientId, clientScopeId, cancellationToken);

    /// <inheritdoc />
    public async Task<List<RoleRepresentation>> GetClientRoles(string realm, string clientId, CancellationToken cancellationToken = default)
        => await _roleContainerApi.GetClientsRolesByIdAsync(realm, clientId, cancellationToken: cancellationToken);

    /// <inheritdoc />
    public async Task CreateClientRole(string realm, string clientId, RoleRepresentation role, CancellationToken cancellationToken = default)
        => await _roleContainerApi.PostClientsRolesByIdAsync(realm, clientId, role, cancellationToken);

    /// <inheritdoc />
    public async Task<List<UserRepresentation>> GetUsers(string realm, string search = default, string lastName = default, string firstName = default, string email = default, string username = default, bool? emailVerified = default, string idpAlias = default, string idpUserId = default, int? first = default, int? max = default, bool? enabled = default, bool? briefRepresentation = default, bool? exact = default, string q = default, CancellationToken cancellationToken = default)
        => await _usersService.GetUsersAsync(realm, search, lastName, firstName, email, username, emailVerified, idpAlias, idpUserId, first, max, enabled, briefRepresentation, exact, q, cancellationToken);

    /// <inheritdoc />
    public async Task CreateUser(string realm, UserRepresentation user, CancellationToken cancellationToken)
        => await _usersService.PostUsersAsync(realm, user, cancellationToken);

    /// <inheritdoc />
    public async Task AddClientRolesToUser(string realm, string userId, string clientId, List<RoleRepresentation> roles, CancellationToken cancellationToken)
        => await _clientRoleMappingsApi.PostUsersRoleMappingsClientsByIdAndClientAsync(realm, userId, clientId, roles, cancellationToken);

    /// <inheritdoc />
    public async Task DeleteClientRolesOfUser(string realm, string userId, string clientId, List<RoleRepresentation> roles, CancellationToken cancellationToken = default)
        => await _clientRoleMappingsApi.DeleteUsersRoleMappingsClientsByIdAndClientAsync(realm, userId, clientId, roles, cancellationToken);
}