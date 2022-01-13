﻿using FS.FilterExpressionCreator.Filters;
using FS.TimeTracking.Api.REST.Routing;
using FS.TimeTracking.Shared.DTOs.MasterData;
using FS.TimeTracking.Shared.DTOs.Report;
using FS.TimeTracking.Shared.DTOs.TimeTracking;
using FS.TimeTracking.Shared.Interfaces.Application.Services.Report;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Api.REST.Controllers.Report;

/// <inheritdoc cref="ICustomerReportService" />
/// <seealso cref="ControllerBase" />
/// <seealso cref="ICustomerReportService" />
[V1ApiController]
public class CustomerReportController : ControllerBase, ICustomerReportService
{
    private readonly ICustomerReportService _reportService;

    /// <summary>
    /// Initializes a new instance of the <see cref="OrderReportController"/> class.
    /// </summary>
    /// <param name="reportService">The report service.</param>
    public CustomerReportController(ICustomerReportService reportService)
        => _reportService = reportService;

    /// <inheritdoc />
    [HttpGet]
    public async Task<List<CustomerWorkTimeDto>> GetWorkTimesPerCustomer([FromQuery] EntityFilter<TimeSheetDto> timeSheetFilter, [FromQuery] EntityFilter<ProjectDto> projectFilter, [FromQuery] EntityFilter<CustomerDto> customerFilter, [FromQuery] EntityFilter<ActivityDto> activityFilter, [FromQuery] EntityFilter<OrderDto> orderFilter, [FromQuery] EntityFilter<HolidayDto> holidayFilter, CancellationToken cancellationToken = default)
        => await _reportService.GetWorkTimesPerCustomer(timeSheetFilter, projectFilter, customerFilter, activityFilter, orderFilter, holidayFilter, cancellationToken);
}