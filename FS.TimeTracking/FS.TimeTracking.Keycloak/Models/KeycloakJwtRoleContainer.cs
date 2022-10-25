// ReSharper disable StringLiteralTypo
// 

namespace FS.TimeTracking.Keycloak.Models;

/// <summary>
/// Authorization roles container.
/// </summary>
public class KeycloakJwtRoleContainer
{
    /// <summary>
    /// Gets or sets the assigned roles.
    /// </summary>
    public string[] Roles { get; set; }
}
