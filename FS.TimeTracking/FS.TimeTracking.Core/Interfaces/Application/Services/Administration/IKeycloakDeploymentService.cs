using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Core.Interfaces.Application.Services.Authorization;

/// <summary>
/// Keycloak deployment service
/// </summary>
public interface IKeycloakDeploymentService
{
    /// <summary>
    /// Creates the realm and all it's stuff required to use authentication / authorization.
    /// </summary>
    /// <param name="cancellationToken">A token that allows processing to be cancelled.</param>
    Task<bool> CreateRealmIfNotExists(CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes the realm if exists.
    /// </summary>
    /// <param name="cancellationToken">a token that allows processing to be cancelled.</param>
    Task DeleteRealmIfExists(CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets the user identifier of related entities to default user.
    /// </summary>
    /// <param name="cancellationToken">a token that allows processing to be cancelled.</param>
    Task SetUserIdOfRelatedEntitiesToDefaultUser(CancellationToken cancellationToken = default);
}