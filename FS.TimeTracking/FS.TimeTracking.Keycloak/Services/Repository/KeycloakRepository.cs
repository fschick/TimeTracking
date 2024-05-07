using FS.Keycloak.RestApiClient.Api;
using FS.Keycloak.RestApiClient.Model;
using FS.TimeTracking.Core.Interfaces.Repository.Services.Administration;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Keycloak.Services.Repository;

/// <inheritdoc />
public class KeycloakRepository : IKeycloakRepository
{
    private readonly IRealmsAdminApi _realmsService;
    private readonly IClientsApi _clientsService;
    private readonly IClientScopesApi _clientScopesService;
    private readonly IRolesApi _rolesApi;
    private readonly IUsersApi _usersService;
    private readonly IClientRoleMappingsApi _clientRoleMappingsApi;
    private readonly IComponentApi _componentApi;

    /// <summary>
    /// Initializes a new instance of the <see cref="KeycloakRepository"/> class.
    /// </summary>
    public KeycloakRepository(IRealmsAdminApi realmsService, IClientsApi clientsService, IClientScopesApi clientScopesService, IRolesApi rolesApi, IUsersApi usersService, IClientRoleMappingsApi clientRoleMappingsApi, IComponentApi componentApi)
    {
        _realmsService = realmsService;
        _clientsService = clientsService;
        _clientScopesService = clientScopesService;
        _rolesApi = rolesApi;
        _usersService = usersService;
        _clientRoleMappingsApi = clientRoleMappingsApi;
        _componentApi = componentApi;
    }

    /// <inheritdoc />
    public async Task<List<RealmRepresentation>> GetRealms(CancellationToken cancellationToken = default)
        => await _realmsService.GetAsync(cancellationToken: cancellationToken);

    /// <inheritdoc />
    public async Task CreateRealm(RealmRepresentation realm, CancellationToken cancellationToken = default)
    => await _realmsService.PostAsync(realm, cancellationToken);

    /// <inheritdoc />
    public async Task DeleteRealm(string realm, CancellationToken cancellationToken = default)
        => await _realmsService.DeleteAsync(realm, cancellationToken);

    /// <inheritdoc />
    public async Task<List<ComponentRepresentation>> GetComponents(string realm, CancellationToken cancellationToken = default)
        => await _componentApi.GetComponentsAsync(realm, cancellationToken: cancellationToken);

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
    public async Task AddScopeToClient(string realm, string clientId, string clientScopeId, CancellationToken cancellationToken = default)
        => await _clientsService.PutClientsDefaultClientScopesByClientUuidAndClientScopeIdAsync(realm, clientId, clientScopeId, cancellationToken);

    /// <inheritdoc />
    public async Task<List<RoleRepresentation>> GetClientRoles(string realm, string clientId, CancellationToken cancellationToken = default)
        => await _rolesApi.GetClientsRolesByClientUuidAsync(realm, clientId, cancellationToken: cancellationToken);

    /// <inheritdoc />
    public async Task CreateClientRole(string realm, string clientId, RoleRepresentation role, CancellationToken cancellationToken = default)
        => await _rolesApi.PostClientsRolesByClientUuidAsync(realm, clientId, role, cancellationToken);

    /// <inheritdoc />
    public async Task DeleteClientRole(string realm, string clientId, RoleRepresentation role, CancellationToken cancellationToken = default)
        => await _rolesApi.DeleteClientsRolesByClientUuidAndRoleNameAsync(realm, clientId, role.Id, cancellationToken);

    /// <inheritdoc />
    public async Task<List<UserRepresentation>> GetUsers(string realm, bool? briefRepresentation = null, string email = null, bool? emailVerified = null, bool? enabled = null, bool? exact = null, int? first = null, string firstName = null, string idpAlias = null, string idpUserId = null, string lastName = null, int? max = null, string q = null, string search = null, string username = null, CancellationToken cancellationToken = default)
        => await _usersService.GetUsersAsync(realm, briefRepresentation, email, emailVerified, enabled, exact, first, firstName, idpAlias, idpUserId, lastName, max, q, search, username, cancellationToken);

    /// <inheritdoc />
    public async Task<UserRepresentation> GetUser(string realm, Guid userId, CancellationToken cancellationToken = default)
        => await _usersService.GetUsersByUserIdAsync(realm, userId.ToString(), cancellationToken: cancellationToken);

    /// <inheritdoc />
    public async Task CreateUser(string realm, UserRepresentation user, CancellationToken cancellationToken = default)
        => await _usersService.PostUsersAsync(realm, user, cancellationToken);

    /// <inheritdoc />
    public async Task UpdateUser(string realm, UserRepresentation user, CancellationToken cancellationToken = default)
        => await _usersService.PutUsersByUserIdAsync(realm, user.Id, user, cancellationToken);

    /// <inheritdoc />
    public async Task DeleteUser(string realm, Guid userId, CancellationToken cancellationToken = default)
        => await _usersService.DeleteUsersByUserIdAsync(realm, userId.ToString(), cancellationToken);

    /// <inheritdoc />
    public async Task<List<RoleRepresentation>> GetClientRolesOfUser(string realm, Guid userId, string clientId, CancellationToken cancellationToken = default)
        => await _clientRoleMappingsApi.GetUsersRoleMappingsClientsByUserIdAndClientAsync(realm, userId.ToString(), clientId, cancellationToken);

    /// <inheritdoc />
    public async Task AddClientRolesToUser(string realm, Guid userId, string clientId, List<RoleRepresentation> roles, CancellationToken cancellationToken = default)
        => await _clientRoleMappingsApi.PostUsersRoleMappingsClientsByUserIdAndClientAsync(realm, userId.ToString(), clientId, roles, cancellationToken);

    /// <inheritdoc />
    public async Task DeleteClientRolesOfUser(string realm, Guid userId, string clientId, List<RoleRepresentation> roles, CancellationToken cancellationToken = default)
        => await _clientRoleMappingsApi.DeleteUsersRoleMappingsClientsByUserIdAndClientAsync(realm, userId.ToString(), clientId, roles, cancellationToken);
}