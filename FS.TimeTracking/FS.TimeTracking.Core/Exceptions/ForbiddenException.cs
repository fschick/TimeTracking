﻿using FS.TimeTracking.Core.Models.Application.Core;

namespace FS.TimeTracking.Core.Exceptions;

/// <summary>
/// Exception for signalling user is not allowed to execute requested operation.
/// </summary>
public class ForbiddenException : ApplicationErrorException
{
    /// <inheritdoc />
    public ForbiddenException(params string[] errors)
        : base(ApplicationErrorCode.Forbidden, errors) { }

    /// <inheritdoc />
    public ForbiddenException(ApplicationErrorCode errorCode, params string[] errors)
        : base(errorCode, errors) { }
}