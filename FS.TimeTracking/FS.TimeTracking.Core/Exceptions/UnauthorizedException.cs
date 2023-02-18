using FS.TimeTracking.Core.Models.Application.Core;

namespace FS.TimeTracking.Core.Exceptions;

/// <summary>
/// Exception for signalling user is not allowed to execute requested operation.
/// </summary>
public class UnauthorizedException : ApplicationErrorException
{
    /// <inheritdoc />
    public UnauthorizedException(params string[] errors)
        : base(ApplicationErrorCode.Unauthorized, errors) { }

    /// <inheritdoc />
    public UnauthorizedException(ApplicationErrorCode errorCode, params string[] errors)
        : base(errorCode, errors) { }
}