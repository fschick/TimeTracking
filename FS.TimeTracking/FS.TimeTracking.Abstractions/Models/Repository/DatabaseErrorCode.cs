namespace FS.TimeTracking.Abstractions.Models.Repository;

/// <summary>
/// Unified database error codes.
/// </summary>
public enum DatabaseErrorCode
{
    /// <summary>
    /// An unknown error has occurred.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// A foreign key violation has occurred.
    /// </summary>
    ForeignKeyViolation = 1
}