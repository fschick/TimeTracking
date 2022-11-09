﻿using FS.TimeTracking.Abstractions.Constants;
using FS.TimeTracking.Abstractions.DTOs.Administration;
using FS.TimeTracking.Api.REST.Filters;
using FS.TimeTracking.Api.REST.Models;
using FS.TimeTracking.Api.REST.Routing;
using FS.TimeTracking.Core.Interfaces.Application.Services.Administration;
using FS.TimeTracking.Core.Interfaces.Application.Services.MasterData;
using FS.TimeTracking.Core.Models.Filter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Api.REST.Controllers.Administration;

/// <inheritdoc cref="IActivityService" />
/// <seealso cref="ControllerBase" />
/// <seealso cref="IActivityService" />
[ApiV1Controller]
[Authorize]
[FeatureGate(Features.Authorization)]
[ExcludeFromCodeCoverage]
public class UserController : IUserService
{
    private readonly IUserService _userService;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserController"/> class.
    /// </summary>
    /// <param name="userService">The user service.</param>
    /// <autogeneratedoc />
    public UserController(IUserService userService)
        => _userService = userService;

    /// <inheritdoc />
    [HttpGet]
    [NotFoundWhenEmpty]
    [Authorize(Roles = RoleNames.ADMINISTRATION_USERS_VIEW)]
    public async Task<UserDto> Get(Guid id, CancellationToken cancellationToken = default)
        => await _userService.Get(id, cancellationToken);

    /// <inheritdoc />
    [HttpGet]
    [Authorize(Roles = RoleNames.ADMINISTRATION_USERS_VIEW)]
    public async Task<List<UserGridDto>> GetGridFiltered([FromQuery] TimeSheetFilterSet filters, CancellationToken cancellationToken = default)
        => await _userService.GetGridFiltered(filters, cancellationToken);

    /// <inheritdoc />
    [HttpGet]
    [NotFoundWhenEmpty]
    [Authorize(Roles = RoleNames.ADMINISTRATION_USERS_VIEW)]
    public async Task<UserGridDto> GetGridItem(Guid id, CancellationToken cancellationToken = default)
        => await _userService.GetGridItem(id, cancellationToken);

    /// <inheritdoc />
    [HttpPost]
    [Authorize(Roles = RoleNames.ADMINISTRATION_USERS_VIEW)]
    public async Task<UserDto> Create(UserDto dto)
        => await _userService.Create(dto);

    /// <inheritdoc />
    [HttpPut]
    [Authorize(Roles = RoleNames.ADMINISTRATION_USERS_MANAGE)]
    public async Task<UserDto> Update(UserDto dto)
        => await _userService.Update(dto);

    /// <inheritdoc />
    [Authorize(Roles = RoleNames.ADMINISTRATION_USERS_MANAGE)]
    [HttpDelete("{id}", Name = "[controller]_[action]")]
    [ProducesResponseType(typeof(long), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApplicationError), (int)HttpStatusCode.Conflict)]
    public async Task<long> Delete(Guid id)
        => await _userService.Delete(id);
}