﻿using FS.FilterExpressionCreator.Abstractions.Extensions;
using FS.FilterExpressionCreator.Abstractions.Models;
using FS.FilterExpressionCreator.Filters;
using FS.TimeTracking.Application.Extensions;
using FS.TimeTracking.Shared.DTOs.Chart;
using FS.TimeTracking.Shared.DTOs.MasterData;
using FS.TimeTracking.Shared.DTOs.TimeTracking;
using FS.TimeTracking.Shared.Extensions;
using FS.TimeTracking.Shared.Interfaces.Application.Services.Chart;
using FS.TimeTracking.Shared.Interfaces.Application.Services.MasterData;
using FS.TimeTracking.Shared.Interfaces.Repository.Services;
using FS.TimeTracking.Shared.Models.Application.Chart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Application.Services.Chart;

/// <inheritdoc />
public class CustomerChartService : ICustomerChartService
{
    private readonly IOrderChartService _orderChartService;
    private readonly ISettingService _settingService;
    private readonly IRepository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomerChartService" /> class.
    /// </summary>
    /// <param name="orderChartService">The order chart service.</param>
    /// <param name="settingService">The setting service.</param>
    /// <param name="repository">The repository.</param>
    /// <autogeneratedoc />
    public CustomerChartService(IOrderChartService orderChartService, ISettingService settingService, IRepository repository)
    {
        _orderChartService = orderChartService;
        _settingService = settingService;
        _repository = repository;
    }

    /// <inheritdoc />
    public async Task<List<CustomerWorkTimeDto>> GetWorkTimesPerCustomer(EntityFilter<TimeSheetDto> timeSheetFilter, EntityFilter<ProjectDto> projectFilter, EntityFilter<CustomerDto> customerFilter, EntityFilter<ActivityDto> activityFilter, EntityFilter<OrderDto> orderFilter, EntityFilter<HolidayDto> holidayFilter, CancellationToken cancellationToken = default)
    {
        var settings = await _settingService.Get(cancellationToken);
        var filter = ChartFilter.Create(timeSheetFilter, projectFilter, customerFilter, activityFilter, orderFilter, holidayFilter);
        var workedTimesPerCustomer = await GetWorkedTimesPerCustomer(filter, cancellationToken);
        var plannedTimesPerCustomer = await GetPlannedTimesPerCustomer(filter, cancellationToken);

        var totalWorkedDays = workedTimesPerCustomer.Sum(x => x.WorkedDays);
        var totalPlannedDays = plannedTimesPerCustomer.Sum(x => x.PlannedDays);

        var result = workedTimesPerCustomer
            .CrossJoin(
                plannedTimesPerCustomer,
                worked => worked.CustomerId,
                planned => planned.CustomerId,
                (worked, planned) =>
                {
                    if (worked == null && planned == null)
                        throw new InvalidOperationException("Planned and worked entities are null");

                    var plannedTimeSpan = planned != null ? new Section<DateTimeOffset>(planned.PlannedStart, planned.PlannedEnd) : null;
                    return new CustomerWorkTimeDto
                    {
                        CustomerId = worked?.CustomerId ?? planned.CustomerId,
                        CustomerTitle = worked?.CustomerTitle ?? planned?.CustomerTitle,
                        TimeWorked = worked?.WorkedTime ?? TimeSpan.Zero,
                        DaysWorked = worked?.WorkedDays ?? 0,
                        RatioTotalWorked = totalWorkedDays != 0 ? (worked?.WorkedDays ?? 0) / totalWorkedDays : 0,
                        BudgetWorked = worked?.WorkedBudget ?? 0,
                        TimePlanned = planned?.PlannedTime,
                        DaysPlanned = planned?.PlannedDays,
                        RatioTotalPlanned = totalPlannedDays != 0 ? (planned?.PlannedDays ?? 0) / totalPlannedDays : null,
                        BudgetPlanned = planned?.PlannedBudget,
                        PlannedStart = planned?.PlannedStart,
                        PlannedEnd = planned?.PlannedEnd,
                        PlannedIsPartial = plannedTimeSpan != null && !filter.SelectedPeriod.Contains(plannedTimeSpan),
                        PlannedHourlyRate = planned?.HourlyRate,
                        Currency = settings.Currency,
                    };
                })
            .OrderBy(x => x.PlannedStart == null)
            .ThenBy(x => x.PlannedStart)
            .ThenBy(x => x.CustomerTitle)
            .ToList();

        return result;
    }

    private async Task<List<CustomerWorkTime>> GetWorkedTimesPerCustomer(ChartFilter filter, CancellationToken cancellationToken)
    {
        var settings = await _settingService.Get(cancellationToken);
        var workedTimesWithOrder = await GetWorkedTimesWithOrderPerCustomer(filter, cancellationToken);
        var workedTimesWithoutOrder = await GetWorkedTimesWithoutOrderPerCustomer(filter, cancellationToken);

        var workedTimesPerCustomer = workedTimesWithOrder
            .CrossJoin(
                workedTimesWithoutOrder,
                x => x.CustomerId,
                x => x.CustomerId,
                (withOrder, withoutOrder) =>
                {
                    var workTimes = new[] { withOrder, withoutOrder }.Where(x => x != null).ToList();
                    return new CustomerWorkTime
                    {
                        CustomerId = withOrder?.CustomerId ?? withoutOrder.CustomerId,
                        CustomerTitle = withOrder?.CustomerTitle ?? withoutOrder.CustomerTitle,
                        WorkedTime = workTimes.Sum(x => x.WorkedTime),
                        HourlyRate = workTimes.GetAverageHourlyRate(x => x?.WorkedTime),
                    };
                }
            )
            .ToList();

        foreach (var workTime in workedTimesPerCustomer)
            workTime.WorkedDays = workTime.WorkedTime.TotalHours / settings.WorkHoursPerWorkday.TotalHours;

        return workedTimesPerCustomer;
    }

    private async Task<List<CustomerWorkTime>> GetWorkedTimesWithOrderPerCustomer(ChartFilter filter, CancellationToken cancellationToken)
    {
        var workedTimesPerOrder = await _orderChartService.GetWorkedTimesPerOrder(filter, cancellationToken);
        var workTimesWithOrder = workedTimesPerOrder
            .GroupBy(x => x.CustomerId)
            .Select(orderWorkItems => new CustomerWorkTime
            {
                CustomerId = orderWorkItems.Key,
                CustomerTitle = orderWorkItems.First().CustomerTitle,
                WorkedTime = orderWorkItems.Sum(x => x.WorkedTime),
                HourlyRate = orderWorkItems.ToList().GetAverageHourlyRate(x => x?.WorkedTime)
            })
            .ToList();
        return workTimesWithOrder;
    }

    private async Task<List<CustomerWorkTime>> GetWorkedTimesWithoutOrderPerCustomer(ChartFilter filter, CancellationToken cancellationToken)
        => await _repository
            .GetGrouped(
                groupBy: x => new { x.Project.Customer.Id, x.Project.Customer.Title },
                @select: x => new CustomerWorkTime
                {
                    CustomerId = x.Key.Id,
                    CustomerTitle = x.Key.Title,
                    WorkedTime = TimeSpan.FromSeconds(x.Sum(f => (double)f.StartDateLocal.DiffSeconds(f.StartDateOffset, f.EndDateLocal))),
                    HourlyRate = x.Min(t => t.Project.Customer.HourlyRate),
                },
                @where: new[] { filter.WorkedTimes.CreateFilter(), x => x.OrderId == null }.CombineWithConditionalAnd(),
                cancellationToken: cancellationToken
            );

    private async Task<List<CustomerWorkTime>> GetPlannedTimesPerCustomer(ChartFilter filter, CancellationToken cancellationToken)
    {
        var settings = await _settingService.Get(cancellationToken);

        var plannedTimesPerOrder = await _orderChartService.GetPlannedTimesPerOrder(filter, cancellationToken);
        var plannedTimesPerCustomer = plannedTimesPerOrder
            .GroupBy(x => x.CustomerId)
            .Select(orderWorkItems => new CustomerWorkTime
            {
                CustomerId = orderWorkItems.Key,
                CustomerTitle = orderWorkItems.First().CustomerTitle,
                PlannedTime = orderWorkItems.Sum(x => x.PlannedTime),
                HourlyRate = orderWorkItems.ToList().GetAverageHourlyRate(x => x?.PlannedTime),
                PlannedStart = orderWorkItems.Min(x => x.PlannedStart),
                PlannedEnd = orderWorkItems.Max(x => x.PlannedEnd)
            })
            .ToList();

        foreach (var workTime in plannedTimesPerCustomer)
            workTime.PlannedDays = workTime.PlannedTime.TotalHours / settings.WorkHoursPerWorkday.TotalHours;

        return plannedTimesPerCustomer;
    }
}