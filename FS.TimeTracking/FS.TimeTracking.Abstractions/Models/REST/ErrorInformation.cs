using FS.TimeTracking.Abstractions.Models.Repository;

namespace FS.TimeTracking.Abstractions.Models.REST;

/// <summary>
/// Contains extended error information about failed API requests
/// </summary>
public class ErrorInformation
{
    /// <summary>
    /// A unified database error code.
    /// </summary>
    public DatabaseErrorCode DatabaseErrorCode { get; set; }
}