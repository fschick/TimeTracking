using FS.Keycloak.RestApiClient.Model;
using FS.TimeTracking.Abstractions.Constants;
using FS.TimeTracking.Abstractions.DTOs.Administration;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace FS.TimeTracking.Application.Tests.Services.FakeModels;

[ExcludeFromCodeCoverage]
public class FakeUser
{
    private readonly Faker _faker;

    public FakeUser(Faker faker)
        => _faker = faker;

    public UserDto Create(string prefix = "Test", bool hidden = false, IEnumerable<PermissionDto> permissions = null, IEnumerable<Guid> restrictToCustomers = null)
    {
        permissions ??= DefaultPermissions.NoPermissions;

        return new()
        {
            Id = _faker.Guid.Create(),
            Username = $"{prefix}{nameof(UserRepresentation.Username)}",
            FirstName = $"{prefix}{nameof(UserRepresentation.FirstName)}",
            LastName = $"{prefix}{nameof(UserRepresentation.LastName)}",
            Enabled = !hidden,
            Permissions = permissions.ToList(),
            RestrictToCustomerIds = restrictToCustomers?.ToList() ?? new List<Guid>()
        };
    }

    public UserDto CreateDto(string prefix = "Test", bool hidden = false, IEnumerable<PermissionDto> permissions = null, IEnumerable<Guid> restrictToCustomers = null)
        => Create(prefix, hidden, permissions);
}
