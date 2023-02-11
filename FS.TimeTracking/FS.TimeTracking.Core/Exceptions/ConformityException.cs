using FS.TimeTracking.Core.Models.Application.Core;
using System.Collections.Generic;

namespace FS.TimeTracking.Core.Exceptions;

/// <summary>
/// Exception for signalling conflicts when model is added or updated to database.
/// </summary>
public class ConformityException : ApplicationErrorException
{
    /// <inheritdoc />
    public ConformityException(params string[] errors)
        : base(errors) { }

    /// <inheritdoc />
    public ConformityException(IEnumerable<string> errors)
        : base(errors) { }

    /// <inheritdoc />
    public ConformityException(ApplicationErrorCode errorCode, params string[] errors)
        : base(errorCode, errors) { }
}