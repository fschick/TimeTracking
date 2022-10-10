namespace FS.TimeTracking.Core.Models.REST;

/// <summary>
/// Unified database error codes.
/// </summary>
public enum RestErrorCode
{
    /// <summary>
    /// An unknown error has occurred.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// Requested resource could not found.
    /// </summary>
    NotFound = 404,

    /// <summary>
    /// The server encountered an unexpected condition that prevented it from fulfilling the request.
    /// </summary>
    InternalServerError = 500,

    /// <summary>
    /// A foreign key violation has occurred.
    /// </summary>
    ForeignKeyViolation = 1100,

    /// <summary>
    /// A foreign key violation has occurred during DELETE action.
    /// </summary>
    ForeignKeyViolationOnDelete = 1101,
}