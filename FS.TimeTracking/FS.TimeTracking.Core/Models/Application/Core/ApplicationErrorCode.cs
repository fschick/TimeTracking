namespace FS.TimeTracking.Core.Models.Application.Core;

/// <summary>
/// Application specific error codes.
/// </summary>
public enum ApplicationErrorCode
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

    /// <summary>
    /// A conflict has occurred while model was added or updated to database.
    /// </summary>
    ConformityViolation = 2000,

    /// <summary>
    /// Activity is already assigned to time sheets with different customers.
    /// </summary>
    ConformityViolationActivityAlreadyAssignedToDifferentCustomers = 2001,

    /// <summary>
    /// Activity is already assigned to time sheets with different projects.
    /// </summary>
    ConformityViolationActivityAlreadyAssignedToDifferentProjects = 2002,
}