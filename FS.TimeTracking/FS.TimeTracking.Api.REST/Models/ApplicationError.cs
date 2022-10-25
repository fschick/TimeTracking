using FS.TimeTracking.Core.Models.Application.Core;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FS.TimeTracking.Api.REST.Models;

/// <summary>
/// Contains extended error information about failed API requests
/// </summary>
[ExcludeFromCodeCoverage]
public class ApplicationError
{
    /// <summary>
    /// The error code.
    /// </summary>
    [Required]
    public ApplicationErrorCode Code { get; set; }

    /// <summary>
    /// Gets detailed causes of the error.
    /// </summary>
    [Required]
    public IEnumerable<string> Messages { get; set; } = new List<string>();
}