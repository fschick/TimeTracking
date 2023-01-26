using FS.Keycloak.RestApiClient.Model;
using FS.TimeTracking.Abstractions.DTOs.Administration;
using System.Diagnostics.CodeAnalysis;

namespace FS.TimeTracking.Application.Tests.Services;

[ExcludeFromCodeCoverage]
public class FakeUser
{
    private readonly Faker _faker;

    public FakeUser(Faker faker)
        => _faker = faker;

    public UserDto Create(string prefix = "Test", bool hidden = false)
        => new()
        {
            Id = _faker.Guid.Create(),
            Username = $"{prefix}{nameof(UserRepresentation.Username)}",
            FirstName = $"{prefix}{nameof(UserRepresentation.FirstName)}",
            LastName = $"{prefix}{nameof(UserRepresentation.LastName)}",
            Enabled = !hidden
        };

    public UserDto CreateDto(string prefix = "Test", bool hidden = false)
        => Create(prefix, hidden);
}