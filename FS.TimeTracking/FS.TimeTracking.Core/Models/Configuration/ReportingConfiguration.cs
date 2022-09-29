using System.Diagnostics.CodeAnalysis;

namespace FS.TimeTracking.Core.Models.Configuration;

/// <summary>
/// Report specific configuration.
/// </summary>
[ExcludeFromCodeCoverage]
public class ReportingConfiguration
{
    /// <summary>
    /// Gets or sets the base URL of the report server.
    /// </summary>
    public string ReportServerBaseUrl { get; set; }
}