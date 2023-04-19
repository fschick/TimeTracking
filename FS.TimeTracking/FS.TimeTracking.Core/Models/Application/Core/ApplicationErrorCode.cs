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
    /// The request is malformed.
    /// </summary>
    BadRequest = 400,

    /// <summary>
    /// Access to the requested resource requires authentication.
    /// </summary>
    Unauthorized = 401,

    /// <summary>
    /// Access to the requested resource is not authorized.
    /// </summary>
    Forbidden = 403,

    /// <summary>
    /// The requested resource could not found.
    /// </summary>
    NotFound = 404,

    /// <summary>
    /// The request conflicts with the current state.
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
    /// Current user is not allowed to view or modify data from other user.
    /// </summary>
    ForbiddenForeignUserData = 40301,

    /// <summary>
    /// Current user is not allowed to view or modify data of related customer.
    /// </summary>
    ForbiddenRestrictedCustomer = 40302,

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

    /// <summary>
    /// A user with same user name or email already exists.
    /// </summary>
    ConflictUserWithSameUsernameExists = 40905,

    /// <summary>
    /// A user is not allowed to delete itself.
    /// </summary>
    ConflictUserNotAllowedToDeleteItself = 40906,

    /// <summary>
    /// A user is not allowed to remove the user edit permission from itself.
    /// </summary>
    ConflictUserNotAllowedToRemoveUserEditPermissionFromItself = 40907,
}