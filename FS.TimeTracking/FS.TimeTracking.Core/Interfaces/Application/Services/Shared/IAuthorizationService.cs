namespace FS.TimeTracking.Core.Interfaces.Application.Services.Shared;

/// <summary>
/// Service to query / check users privileges.
/// </summary>
public interface IAuthorizationService
{
    /// <summary>
    /// Indicates whether the entity (user) represented by current claims principal is in the specified role.
    /// </summary>
    bool IsCurrentUserInRole(string role);
}