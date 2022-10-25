﻿using FS.TimeTracking.Abstractions.Constants;
using FS.TimeTracking.Abstractions.DTOs.MasterData;
using FS.TimeTracking.Abstractions.Enums;
using FS.TimeTracking.Api.REST.Controllers.Shared;
using FS.TimeTracking.Api.REST.Routing;
using FS.TimeTracking.Core.Interfaces.Application.Services.MasterData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Api.REST.Controllers.MasterData;

/// <inheritdoc cref="IHolidayService" />
/// <seealso cref="ControllerBase" />
/// <seealso cref="IHolidayService" />
[ApiV1Controller]
[Authorize(Policy = PermissionNames.MASTER_DATA_HOLIDAYS)]
[ExcludeFromCodeCoverage]
public class HolidayController : CrudModelController<HolidayDto, HolidayGridDto>, IHolidayService
{
    private readonly IHolidayService _holidayService;

    /// <summary>
    /// Initializes a new instance of the <see cref="HolidayController"/> class.
    /// </summary>
    /// <param name="modelService">The model service.</param>
    public HolidayController(IHolidayService modelService)
        : base(modelService)
    {
        _holidayService = modelService;
    }

    /// <inheritdoc />
    [HttpPost]
    public async Task Import([Required] IFormFile file, [Required] HolidayType type, CancellationToken cancellationToken = default)
        => await _holidayService.Import(file, type, cancellationToken);
}