using System.ComponentModel.DataAnnotations;

namespace FS.TimeTracking.Abstractions.DTOs.Configuration;

/// <summary>
/// Client configuration.
/// </summary>
public class ClientConfigurationDto
{
    /// <inheritdoc cref="FeatureConfigurationDto"/>
    [Required]
    public FeatureConfigurationDto Features { get; set; } = new();

    /// <inheritdoc cref="KeycloakConfigurationDto"/>
    [Required]
    public KeycloakConfigurationDto Keycloak { get; set; } = new();
}