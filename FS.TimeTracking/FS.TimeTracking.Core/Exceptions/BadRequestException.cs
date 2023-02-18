using FS.TimeTracking.Core.Models.Application.Core;

namespace FS.TimeTracking.Core.Exceptions;

/// <summary>
/// Exception for signalling when the model is malformed.
/// </summary>
public class BadRequestException : ApplicationErrorException
{
    /// <inheritdoc />
    public BadRequestException(params string[] errors)
        : base(ApplicationErrorCode.BadRequest, errors) { }

    /// <inheritdoc />
    public BadRequestException(ApplicationErrorCode errorCode, params string[] errors)
        : base(errorCode, errors) { }
}