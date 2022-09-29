using FS.TimeTracking.Core.Models.Repository;
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
    public DatabaseErrorCode DatabaseErrorCode { get; set; }
}