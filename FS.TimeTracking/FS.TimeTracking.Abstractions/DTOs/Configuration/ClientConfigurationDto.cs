using System.ComponentModel.DataAnnotations;

namespace FS.TimeTracking.Abstractions.DTOs.Configuration;

/// <summary>
/// Client configuration.
/// </summary>
public class ClientConfigurationDto
{
    /// <inheritdoc cref="ClientFeaturesDto"/>
    [Required]
    public ClientFeaturesDto Features { get; set; } = new();
}