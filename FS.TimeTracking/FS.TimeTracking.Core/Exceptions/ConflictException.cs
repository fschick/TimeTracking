using FS.TimeTracking.Core.Models.Application.Core;
using System.Collections.Generic;

namespace FS.TimeTracking.Core.Exceptions;

/// <summary>
/// Exception for signalling conflicts when model is added or updated to database.
/// </summary>
public class ConflictException : ApplicationErrorException
{
    /// <inheritdoc />
    public ConflictException(params string[] errors)
        : base(errors) { }

    /// <inheritdoc />
    public ConflictException(IEnumerable<string> errors)
        : base(errors) { }

    /// <inheritdoc />
    public ConflictException(ApplicationErrorCode errorCode, params string[] errors)
        : base(errorCode, errors) { }
}