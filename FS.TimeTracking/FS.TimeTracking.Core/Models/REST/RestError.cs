using System.Collections.Generic;
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
    /// A unified error code.
    /// </summary>
    [Required]
    public RestErrorCode Code { get; set; }

    /// <summary>
    /// Gets detailed causes of the error.
    /// </summary>
    [Required]
    public IEnumerable<string> Messages { get; set; } = new List<string>();
}