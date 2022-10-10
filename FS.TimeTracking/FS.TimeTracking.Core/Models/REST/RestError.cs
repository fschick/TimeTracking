using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FS.TimeTracking.Core.Models.REST;

/// <summary>
/// Contains extended error information about failed API requests
/// </summary>
[ExcludeFromCodeCoverage]
public class RestError
{
    /// <summary>
    /// A unified database error code.
    /// </summary>
    [Required]
    public RestErrorCode ErrorCode { get; set; }
}