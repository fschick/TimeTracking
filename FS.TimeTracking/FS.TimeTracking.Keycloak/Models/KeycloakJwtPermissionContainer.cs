// ReSharper disable StringLiteralTypo
// 

namespace FS.TimeTracking.Keycloak.Models;

/// <summary>
/// UMA permission container.
/// </summary>
public class KeycloakJwtPermissionContainer
{
    /// <summary>
    /// Gets or sets the permissions.
    /// </summary>
    public KeycloakJwtPermission[] Permissions { get; set; }
}
