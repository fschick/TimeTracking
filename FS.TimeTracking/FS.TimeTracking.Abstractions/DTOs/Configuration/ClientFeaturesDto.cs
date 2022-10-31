using System.ComponentModel.DataAnnotations;

namespace FS.TimeTracking.Abstractions.DTOs.Configuration;

/// <summary>
/// Feature management.
/// </summary>
public class ClientFeaturesDto
{
    /// <summary>
    /// Enable / disable reporting module.
    /// </summary>
    [Required]
    public bool Reporting { get; set; }
}