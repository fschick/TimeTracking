using FS.TimeTracking.Abstractions.DTOs.Administration;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FS.TimeTracking.Abstractions.DTOs.Configuration;

/// <summary>
/// Client configuration.
/// </summary>
public record ClientConfigurationDto
{
    /// <inheritdoc cref="FeatureConfigurationDto"/>
    [Required]
    public FeatureConfigurationDto Features { get; set; } = new();

    /// <inheritdoc cref="KeycloakConfigurationDto"/>
    [Required]
    public KeycloakConfigurationDto Keycloak { get; set; } = new();

    /// <summary>
    /// Default permissions.
    /// </summary>
    [Required]
    public List<PermissionDto> DefaultPermissions { get; set; } = new();
}