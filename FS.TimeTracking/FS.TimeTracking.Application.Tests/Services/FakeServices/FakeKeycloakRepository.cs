using FS.Keycloak.RestApiClient.Client;
using FS.Keycloak.RestApiClient.Model;
using FS.TimeTracking.Abstractions.Constants;
using FS.TimeTracking.Core.Interfaces.Repository.Services.Administration;
using FS.TimeTracking.Core.Models.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Application.Tests.Services.FakeServices;

public class FakeKeycloakRepository
{
    private readonly Faker _faker;

    public FakeKeycloakRepository(Faker faker)
        => _faker = faker;

    public IKeycloakRepository Create()
    {
        var configuration = _faker.GetRequiredService<IOptions<TimeTrackingConfiguration>>().Value;
        var keycloakClientId = configuration.Keycloak.ClientId;
        return new InMemoryKeycloakRepository(keycloakClientId);
    }

    private class InMemoryKeycloakRepository : IKeycloakRepository
    {
        private readonly ConcurrentDictionary<string, UserRepresentation> _users = new();
        private readonly List<RealmRepresentation> _realms;
        private readonly List<ClientRepresentation> _clients;
        private readonly List<RoleRepresentation> _clientRoles;
        private readonly ConcurrentDictionary<string, List<RoleRepresentation>> _clientUserRoles;

        public InMemoryKeycloakRepository(string clientId)
        {
            _realms = new List<RealmRepresentation> { new() };
            _clients = new List<ClientRepresentation> { new(clientId) };
            _clientUserRoles = new ConcurrentDictionary<string, List<RoleRepresentation>>();
            _clientRoles = RoleName.All
                .Select(role => new RoleRepresentation
                {
                    Name = role,
                    ContainerId = clientId,
                    ClientRole = true,
                })
                .ToList();
        }

        public Task AddClientRolesToUser(string realm, Guid userId, string clientId, List<RoleRepresentation> roles, CancellationToken cancellationToken = default)
        {
            if (!_clientUserRoles.TryGetValue(userId.ToString(), out var userRoles))
                userRoles = new List<RoleRepresentation>();

            userRoles = userRoles.Union(roles).ToList();
            _clientUserRoles.AddOrUpdate(userId.ToString(), userRoles, (_, _) => userRoles);
            return Task.CompletedTask;
        }

        public Task AddScopeToClient(string realm, string clientId, string clientScopeId, CancellationToken cancellationToken = default)
             => throw new NotImplementedException(nameof(AddScopeToClient));

        public Task CreateClient(string realm, ClientRepresentation client, CancellationToken cancellationToken = default)
            => throw new NotImplementedException(nameof(CreateClient));

        public Task CreateClientRole(string realm, string clientId, RoleRepresentation role, CancellationToken cancellationToken = default)
            => throw new NotImplementedException(nameof(CreateClientRole));

        public Task DeleteClientRole(string realm, string clientId, RoleRepresentation role, CancellationToken cancellationToken = default)
            => throw new NotImplementedException(nameof(DeleteClientRole));

        public Task CreateClientScope(string realm, ClientScopeRepresentation clientScope, CancellationToken cancellationToken = default)
            => throw new NotImplementedException(nameof(CreateClientScope));

        public Task CreateRealm(RealmRepresentation realm, CancellationToken cancellationToken = default)
            => throw new NotImplementedException(nameof(CreateRealm));

        public Task CreateUser(string realm, UserRepresentation user, CancellationToken cancellationToken = default)
        {
            _users.AddOrUpdate(user.Id, user, (_, _) => user);
            return Task.CompletedTask;
        }

        public Task DeleteClientRolesOfUser(string realm, Guid userId, string clientId, List<RoleRepresentation> roles, CancellationToken cancellationToken = default)
        {
            if (!_clientUserRoles.TryGetValue(userId.ToString(), out var userRoles))
                return Task.CompletedTask;

            userRoles = userRoles.Except(roles).ToList();
            _clientUserRoles.AddOrUpdate(userId.ToString(), userRoles, (_, _) => userRoles);
            return Task.CompletedTask;
        }

        public Task DeleteRealm(string realm, CancellationToken cancellationToken = default)
             => throw new NotImplementedException(nameof(DeleteRealm));

        public Task<List<ComponentRepresentation>> GetComponents(string realm, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task DeleteUser(string realm, Guid userId, CancellationToken cancellationToken = default)
        {
            if (!_users.TryRemove(userId.ToString(), out _))
                throw new ApiException(404, "Error calling DeleteUsersById: {\"error\":\"User not found\"}");
            _clientUserRoles.TryRemove(userId.ToString(), out _);
            return Task.CompletedTask;
        }

        public Task<List<RoleRepresentation>> GetClientRoles(string realm, string clientId, CancellationToken cancellationToken = default)
            => Task.FromResult(_clientRoles);

        public Task<List<RoleRepresentation>> GetClientRolesOfUser(string realm, Guid userId, string clientId, CancellationToken cancellationToken = default)
            => Task.FromResult(_clientUserRoles.FirstOrDefault(x => x.Key == userId.ToString()).Value);

        public Task<List<ClientRepresentation>> GetClients(string realm, CancellationToken cancellationToken = default)
            => Task.FromResult(_clients);

        public Task<List<ClientScopeRepresentation>> GetClientScopes(string realm, CancellationToken cancellationToken = default)
             => throw new NotImplementedException(nameof(GetClientScopes));

        public Task<List<RealmRepresentation>> GetRealms(CancellationToken cancellationToken = default)
             => Task.FromResult(_realms);

        public Task<UserRepresentation> GetUser(string realm, Guid userId, CancellationToken cancellationToken = default)
        {
            if (!_users.TryGetValue(userId.ToString(), out var user))
                throw new ApiException(404, "Error calling GetUsersById: {\"error\":\"User not found\"}");
            return Task.FromResult(user);
        }

        public Task<List<UserRepresentation>> GetUsers(string realm, bool? briefRepresentation = null, string email = null, bool? emailVerified = null, bool? enabled = null, bool? exact = null, int? first = null, string firstName = null, string idpAlias = null, string idpUserId = null, string lastName = null, int? max = null, string q = null, string search = null, string username = null, CancellationToken cancellationToken = default)
            => Task.FromResult(_users.Values.ToList());

        public Task UpdateUser(string realm, UserRepresentation user, CancellationToken cancellationToken = default)
        {
            if (!_users.TryGetValue(user.Id, out var currentUser))
                throw new ApiException(404, "Error calling GetUsersById: {\"error\":\"User not found\"}");
            if (!_users.TryUpdate(user.Id, user, currentUser))
                throw new ApiException(404, "Error calling GetUsersById: {\"error\":\"User not found\"}");
            return Task.FromResult(user);
        }
    }

}