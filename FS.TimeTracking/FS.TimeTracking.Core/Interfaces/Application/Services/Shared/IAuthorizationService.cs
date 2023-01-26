﻿using System;
using System.Security.Claims;

namespace FS.TimeTracking.Core.Interfaces.Application.Services.Shared;

/// <summary>
/// Service to query / check users privileges.
/// </summary>
public interface IAuthorizationService
{
    /// <summary>
    /// Gets the current user / identity.
    /// </summary>
    ClaimsPrincipal CurrentUser { get; }

    /// <summary>
    /// Gets the current user identifier.
    /// </summary>
    /// <value>
    /// The identifier of the current user.
    /// </value>
    /// <autogeneratedoc />
    Guid CurrentUserId { get; }
}