using System.Diagnostics.CodeAnalysis;

namespace FS.TimeTracking.Core.Models.REST;

/// <summary>
/// Contains extended error information about failed API requests
/// </summary>
[ExcludeFromCodeCoverage]
public class ErrorInformation
{
    /// <summary>
    /// A unified database error code.
    /// </summary>
    public ErrorCode ErrorCode { get; set; }
}