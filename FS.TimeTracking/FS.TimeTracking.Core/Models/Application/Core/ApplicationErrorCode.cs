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
    /// The request could not be understood by the server due to malformed syntax
    /// </summary>
    BadRequest = 400,

    /// <summary>
    /// Access to requested resource requires authentication.
    /// </summary>
    Unauthorized = 401,

    /// <summary>
    /// Access to requested resource is not authorized.
    /// </summary>
    Forbidden = 403,

    /// <summary>
    /// Requested resource could not found.
    /// </summary>
    NotFound = 404,

    /// <summary>
    /// Request conflicts with the current state.
    /// </summary>
    Conflict = 409,

    /// <summary>
    /// The server encountered an unexpected condition that prevented it from fulfilling the request.
    /// </summary>
    InternalServerError = 500,

    /// <summary>
    /// A conflict has occurred while model was added or updated to database.
    /// </summary>
    BadRequestConformityViolation = 40001,

    /// <summary>
    /// A foreign key violation has occurred.
    /// </summary>
    ConflictForeignKeyViolation = 40901,

    /// <summary>
    /// A foreign key violation has occurred during DELETE action.
    /// </summary>
    ConflictForeignKeyViolationOnDelete = 40902,

    /// <summary>
    /// Activity is already assigned to time sheets with different customers.
    /// </summary>
    ConflictActivityAlreadyAssignedToDifferentCustomers = 40903,

    /// <summary>
    /// Activity is already assigned to time sheets with different projects.
    /// </summary>
    ConflictActivityAlreadyAssignedToDifferentProjects = 40904,
}