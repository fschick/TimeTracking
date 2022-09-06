using System.ComponentModel.DataAnnotations;

namespace FS.TimeTracking.Core.Models.Configuration;

/// <summary>
/// Report specific configuration.
/// </summary>
public class FeatureConfiguration
{
    /// <summary>
    /// Gets or sets if the reporting module is enabled.
    /// </summary>
    [Required]
    public bool Reporting { get; set; }
}