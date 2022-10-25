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