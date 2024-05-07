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
    /// Get all components of realm.
    /// </summary>
    Task<List<ComponentRepresentation>> GetComponents(string realm, CancellationToken cancellationToken = default);

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
    /// <param name="realm">Realm name (not id!).</param>
    /// <param name="clientId">Identifier for the client.</param>
    /// <param name="cancellationToken"> a token that allows processing to be cancelled.</param>
    Task<List<RoleRepresentation>> GetClientRoles(string realm, string clientId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a client role.
    /// </summary>
    /// <param name="realm">Realm name (not id!).</param>
    /// <param name="clientId">Identifier for the client.</param>
    /// <param name="role">The role.</param>
    /// <param name="cancellationToken"> a token that allows processing to be cancelled.</param>
    Task CreateClientRole(string realm, string clientId, RoleRepresentation role, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a client role.
    /// </summary>
    /// <param name="realm">Realm name (not id!).</param>
    /// <param name="clientId">Identifier for the client.</param>
    /// <param name="role">The role.</param>
    /// <param name="cancellationToken"> a token that allows processing to be cancelled.</param>
    Task DeleteClientRole(string realm, string clientId, RoleRepresentation role, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a list of users from a specific realm in Keycloak.
    /// </summary>
    /// <param name="realm">Realm name (not id!).</param>
    /// <param name="briefRepresentation">Defines whether brief representations are returned</param>
    /// <param name="email">A String contained in email, or the complete email, if param &quot;exact&quot; is true.</param>
    /// <param name="emailVerified">Whether the email has been verified.</param>
    /// <param name="enabled">Representing if user is enabled or not.</param>
    /// <param name="exact">Boolean which defines whether the params &quot;last&quot;, &quot;first&quot;, &quot;email&quot; and &quot;username&quot; must match exactly.</param>
    /// <param name="first">Pagination offset.</param>
    /// <param name="firstName">A String contained in firstName, or the complete firstName, if param &quot;exact&quot; is true.</param>
    /// <param name="idpAlias">The alias of an Identity Provider linked to the user.</param>
    /// <param name="idpUserId">The userId at an Identity Provider linked to the user.</param>
    /// <param name="lastName">A String contained in lastName, or the complete lastName, if param &quot;exact&quot; is true.</param>
    /// <param name="max">Maximum results size (defaults to 100).</param>
    /// <param name="q">A query to search for custom attributes, in the format 'key1:value2 key2:value2'.</param>
    /// <param name="search">A String contained in username, first or last name, or email. Default search behavior is prefix-based (e.g., foo or foo*). Use foo for infix search and &quot;foo&quot; for exact search..</param>
    /// <param name="username">A String contained in username, or the complete username, if param &quot;exact&quot; is true.</param>
    /// <param name="cancellationToken">A token that allows the operation to be cancelled.</param>
    Task<List<UserRepresentation>> GetUsers(string realm, bool? briefRepresentation = null, string email = null, bool? emailVerified = null, bool? enabled = null, bool? exact = null, int? first = null, string firstName = null, string idpAlias = null, string idpUserId = null, string lastName = null, int? max = null, string q = null, string search = null, string username = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a user.
    /// </summary>
    /// <param name="realm">Realm name (not id!).</param>
    /// <param name="userId">Identifier for the user.</param>
    /// <param name="cancellationToken">a token that allows processing to be cancelled.</param>
    Task<UserRepresentation> GetUser(string realm, Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a user.
    /// </summary>
    /// <param name="realm">Realm name (not id!).</param>
    /// <param name="user">The user.</param>
    /// <param name="cancellationToken">a token that allows processing to be cancelled.</param>
    Task CreateUser(string realm, UserRepresentation user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a user.
    /// </summary>
    /// <param name="realm">Realm name (not id!).</param>
    /// <param name="user">The user.</param>
    /// <param name="cancellationToken">a token that allows processing to be cancelled.</param>
    Task UpdateUser(string realm, UserRepresentation user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a user.
    /// </summary>
    /// <param name="realm">Realm name (not id!).</param>
    /// <param name="userId">Identifier for the user.</param>
    /// <param name="cancellationToken">a token that allows processing to be cancelled.</param>
    Task DeleteUser(string realm, Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get client roles for user.
    /// </summary>
    /// <param name="realm">Realm name (not id!).</param>
    /// <param name="userId">Identifier for the user.</param>
    /// <param name="clientId">Identifier for the client.</param>
    /// <param name="cancellationToken">a token that allows processing to be cancelled.</param>
    Task<List<RoleRepresentation>> GetClientRolesOfUser(string realm, Guid userId, string clientId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds client roles to user.
    /// </summary>
    /// <param name="realm">Realm name (not id!).</param>
    /// <param name="userId">Identifier for the user.</param>
    /// <param name="clientId">Identifier for the client.</param>
    /// <param name="roles">The roles.</param>
    /// <param name="cancellationToken">a token that allows processing to be cancelled.</param>
    Task AddClientRolesToUser(string realm, Guid userId, string clientId, List<RoleRepresentation> roles, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete client roles from user.
    /// </summary>
    /// <param name="realm">Realm name (not id!).</param>
    /// <param name="userId">Identifier for the user.</param>
    /// <param name="clientId">Identifier for the client.</param>
    /// <param name="roles">The roles.</param>
    /// <param name="cancellationToken">a token that allows processing to be cancelled.</param>
    Task DeleteClientRolesOfUser(string realm, Guid userId, string clientId, List<RoleRepresentation> roles, CancellationToken cancellationToken = default);
}