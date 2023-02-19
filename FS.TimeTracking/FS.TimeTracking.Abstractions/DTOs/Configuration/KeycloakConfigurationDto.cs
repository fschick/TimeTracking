using System.ComponentModel.DataAnnotations;

namespace FS.TimeTracking.Abstractions.DTOs.Configuration;

/// <summary>
///  Keycloak specific configuration.
/// </summary>
public record KeycloakConfigurationDto
{
    /// <summary>
    /// Authorization server URL
    /// </summary>
    [Required]
    public string AuthServerUrl { get; set; }

    /// <summary>
    /// Keycloak Realm
    /// </summary>
    [Required]
    public string Realm { get; set; }

    /// <summary>
    /// Resource as client id
    /// </summary>
    [Required]
    public string ClientId { get; set; }
}