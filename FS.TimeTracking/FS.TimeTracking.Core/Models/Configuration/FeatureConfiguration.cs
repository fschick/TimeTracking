using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FS.TimeTracking.Core.Models.Configuration;

/// <summary>
/// Feature management.
/// </summary>
[ExcludeFromCodeCoverage]
public class FeatureConfiguration
{
    /// <summary>
    /// Gets or sets if the reporting module is enabled.
    /// </summary>
    [Required]
    public bool Reporting { get; set; }
}