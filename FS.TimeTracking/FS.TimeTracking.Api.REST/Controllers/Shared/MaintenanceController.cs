﻿using FS.TimeTracking.Abstractions.Constants;
using FS.TimeTracking.Api.REST.Routing;
using FS.TimeTracking.Core.Interfaces.Application.Services.Administration;
using FS.TimeTracking.Core.Models.Filter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace FS.TimeTracking.Api.REST.Controllers.Shared;

/// <inheritdoc cref="IMaintenanceApiService" />
/// <seealso cref="ControllerBase" />
[ApiV1Controller]
[Authorize]
[ExcludeFromCodeCoverage]
public class MaintenanceController : ControllerBase, IMaintenanceApiService
{
    private readonly IMaintenanceApiService _maintenanceService;

    /// <summary>
    /// Initializes a new instance of the <see cref="MaintenanceController"/> class.
    /// </summary>
    /// <param name="maintenanceService">The test data service.</param>
    /// <autogeneratedoc />
    public MaintenanceController(IMaintenanceApiService maintenanceService)
        => _maintenanceService = maintenanceService;

    /// <inheritdoc />
    [HttpGet]
    [Authorize(Roles = RoleName.ADMINISTRATION_MAINTENANCE_VIEW)]
    public async Task<JObject> ExportData([FromQuery] TimeSheetFilterSet filters)
        => await _maintenanceService.ExportData(filters);

    /// <inheritdoc />
    [HttpPost]
    [Authorize(Roles = RoleName.ADMINISTRATION_MAINTENANCE_MANAGE)]
    public async Task ImportData([FromBody] JObject data)
        => await _maintenanceService.ImportData(data);

    /// <inheritdoc />
    [HttpDelete]
    [Authorize(Roles = RoleName.ADMINISTRATION_MAINTENANCE_MANAGE)]
    public async Task TruncateData()
        => await _maintenanceService.TruncateData();
}