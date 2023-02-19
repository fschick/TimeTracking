using System.ComponentModel.DataAnnotations;

namespace FS.TimeTracking.Abstractions.DTOs.Configuration;

/// <summary>
/// Feature management.
/// </summary>
public record FeatureConfigurationDto
{
    /// <summary>
    /// Enable / disable authentication and authorization using Keycloak.
    /// </summary>
    [Required]
    public bool Authorization { get; set; }

    /// <summary>
    /// Enable / disable reporting module.
    /// </summary>
    [Required]
    public bool Reporting { get; set; }
}