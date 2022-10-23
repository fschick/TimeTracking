﻿using FS.TimeTracking.Core.Models.Application.Core;
using System.Data.Common;

namespace FS.TimeTracking.Core.Interfaces.Repository.Services;

/// <summary>
/// Database specific exception handling
/// </summary>
public interface IDbExceptionService
{
    /// <summary>
    /// Translates the database specific exception to an unified error code.
    /// </summary>
    /// <param name="dbException">The database exception.</param>
    ErrorCode TranslateDbException(DbException dbException);
}