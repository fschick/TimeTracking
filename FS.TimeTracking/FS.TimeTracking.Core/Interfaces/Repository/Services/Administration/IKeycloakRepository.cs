using FS.Keycloak.RestApiClient.Model;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Core.Interfaces.Repository.Services.Administration;

/// <summary>
/// Keycloak repository.
/// </summary>
public interface IKeycloakRepository
{
    /// <summary>
    /// Return all realms.
    /// </summary>
    Task<List<RealmRepresentation>> GetRealms(CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a realm.
    /// </summary>
    Task CreateRealm(RealmRepresentation realm, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a realm.
    /// </summary>
    Task DeleteRealm(string realm, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all clients of realm.
    /// </summary>
    Task<List<ClientRepresentation>> GetClients(string realm, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a client.
    /// </summary>
    Task CreateClient(string realm, ClientRepresentation client, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all client scopes of realm.
    /// </summary>
    Task<List<ClientScopeRepresentation>> GetClientScopes(string realm, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a client scope.
    /// </summary>
    Task CreateClientScope(string realm, ClientScopeRepresentation clientScope, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a client scope to a client.
    /// </summary>
    Task AddScopeToClient(string realm, string clientId, string clientScopeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get client roles.
    /// </summary>
    /// <param name="realm">The realm.</param>
    /// <param name="clientId">Identifier for the client.</param>
    /// <param name="cancellationToken"> a token that allows processing to be cancelled.</param>
    Task<List<RoleRepresentation>> GetClientRoles(string realm, string clientId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a client role.
    /// </summary>
    /// <param name="realm">The realm.</param>
    /// <param name="clientId">Identifier for the client.</param>
    /// <param name="role">The role.</param>
    /// <param name="cancellationToken"> a token that allows processing to be cancelled.</param>
    Task CreateClientRole(string realm, string clientId, RoleRepresentation role, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a client role.
    /// </summary>
    /// <param name="realm">The realm.</param>
    /// <param name="clientId">Identifier for the client.</param>
    /// <param name="role">The role.</param>
    /// <param name="cancellationToken"> a token that allows processing to be cancelled.</param>
    Task DeleteClientRole(string realm, string clientId, RoleRepresentation role, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets users of realm.
    /// </summary>
    /// <param name="realm">Realm.</param>
    /// <param name="search">Search.</param>
    /// <param name="lastName">Person's last name.</param>
    /// <param name="firstName">Person's first name.</param>
    /// <param name="email">E-Mail.</param>
    /// <param name="username">Username.</param>
    /// <param name="emailVerified">True if email verified.</param>
    /// <param name="idpAlias">IDP alias.</param>
    /// <param name="idpUserId">Identifier for the idp user.</param>
    /// <param name="first">First result only.</param>
    /// <param name="max">Maximum results.</param>
    /// <param name="enabled">User enabled.</param>
    /// <param name="briefRepresentation">Brief representation.</param>
    /// <param name="exact">true to get exact match.</param>
    /// <param name="q">Search text.</param>
    /// <param name="cancellationToken">A token that allows processing to be cancelled.</param>
    Task<List<UserRepresentation>> GetUsers(string realm, string search = default, string lastName = default, string firstName = default, string email = default, string username = default, bool? emailVerified = default, string idpAlias = default, Guid? idpUserId = default, int? first = default, int? max = default, bool? enabled = default, bool? briefRepresentation = default, bool? exact = default, string q = default, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a user.
    /// </summary>
    /// <param name="realm">The realm.</param>
    /// <param name="userId">Identifier for the user.</param>
    /// <param name="cancellationToken">a token that allows processing to be cancelled.</param>
    Task<UserRepresentation> GetUser(string realm, Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a user.
    /// </summary>
    /// <param name="realm">The realm.</param>
    /// <param name="user">The user.</param>
    /// <param name="cancellationToken">a token that allows processing to be cancelled.</param>
    Task CreateUser(string realm, UserRepresentation user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a user.
    /// </summary>
    /// <param name="realm">The realm.</param>
    /// <param name="user">The user.</param>
    /// <param name="cancellationToken">a token that allows processing to be cancelled.</param>
    Task UpdateUser(string realm, UserRepresentation user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a user.
    /// </summary>
    /// <param name="realm">The realm.</param>
    /// <param name="userId">Identifier for the user.</param>
    /// <param name="cancellationToken">a token that allows processing to be cancelled.</param>
    Task DeleteUser(string realm, Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get client roles for user.
    /// </summary>
    /// <param name="realm">The realm.</param>
    /// <param name="userId">Identifier for the user.</param>
    /// <param name="clientId">Identifier for the client.</param>
    /// <param name="cancellationToken">a token that allows processing to be cancelled.</param>
    Task<List<RoleRepresentation>> GetClientRolesOfUser(string realm, Guid userId, string clientId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds client roles to user.
    /// </summary>
    /// <param name="realm">The realm.</param>
    /// <param name="userId">Identifier for the user.</param>
    /// <param name="clientId">Identifier for the client.</param>
    /// <param name="roles">The roles.</param>
    /// <param name="cancellationToken">a token that allows processing to be cancelled.</param>
    Task AddClientRolesToUser(string realm, Guid userId, string clientId, List<RoleRepresentation> roles, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete client roles from user.
    /// </summary>
    /// <param name="realm">The realm.</param>
    /// <param name="userId">Identifier for the user.</param>
    /// <param name="clientId">Identifier for the client.</param>
    /// <param name="roles">The roles.</param>
    /// <param name="cancellationToken">a token that allows processing to be cancelled.</param>
    Task DeleteClientRolesOfUser(string realm, Guid userId, string clientId, List<RoleRepresentation> roles, CancellationToken cancellationToken = default);
}