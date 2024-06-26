﻿using FS.TimeTracking.Abstractions.DTOs.Chart;
using FS.TimeTracking.Application.Extensions;
using FS.TimeTracking.Core.Extensions;
using FS.TimeTracking.Core.Interfaces.Application.Services.Administration;
using FS.TimeTracking.Core.Interfaces.Application.Services.Chart;
using FS.TimeTracking.Core.Interfaces.Application.Services.Shared;
using FS.TimeTracking.Core.Interfaces.Repository.Services.Database;
using FS.TimeTracking.Core.Models.Application.Chart;
using FS.TimeTracking.Core.Models.Filter;
using Plainquire.Filter.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Application.Services.Chart;

/// <inheritdoc />
public class CustomerChartService : ICustomerChartApiService
{
    private readonly IOrderChartService _orderChartService;
    private readonly ISettingApiService _settingService;
    private readonly IFilterFactory _filterFactory;
    private readonly IDbRepository _dbRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomerChartService" /> class.
    /// </summary>
    /// <param name="orderChartService">The order chart service.</param>
    /// <param name="settingService">The setting service.</param>
    /// <param name="filterFactory">The filter factory.</param>
    /// <param name="dbRepository">The repository.</param>
    /// <autogeneratedoc />
    public CustomerChartService(IOrderChartService orderChartService, ISettingApiService settingService, IFilterFactory filterFactory, IDbRepository dbRepository)
    {
        _orderChartService = orderChartService;
        _settingService = settingService;
        _filterFactory = filterFactory;
        _dbRepository = dbRepository;
    }

    /// <inheritdoc />
    public async Task<List<CustomerWorkTimeDto>> GetWorkTimesPerCustomer(TimeSheetFilterSet filters, CancellationToken cancellationToken = default)
    {
        var settings = await _settingService.GetSettings(cancellationToken);
        var filter = await _filterFactory.CreateChartFilter(filters);
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

                    var plannedOrderTimeSpan = planned?.PlannedStart.CreateRange(planned.PlannedEnd);
                    var workedOrderTimeSpan = worked?.PlannedStart.CreateRange(worked.PlannedEnd);
                    var plannedTimeSpan = workedOrderTimeSpan.Union(plannedOrderTimeSpan);

                    var daysDifference = planned?.PlannedDays - planned?.WorkedDays;

                    return new CustomerWorkTimeDto
                    {
                        CustomerId = worked?.CustomerId ?? planned.CustomerId,
                        CustomerTitle = worked?.CustomerTitle ?? planned?.CustomerTitle,
                        CustomerHidden = worked?.CustomerHidden ?? planned.CustomerHidden,
                        TotalWorkedPercentage = totalWorkedDays != 0 ? (worked?.WorkedDays ?? 0) / totalWorkedDays : 0,
                        TotalPlannedPercentage = totalPlannedDays != 0 ? (planned?.PlannedDays ?? 0) / totalPlannedDays : null,
                        DaysWorked = worked?.WorkedDays ?? 0,
                        DaysPlanned = planned?.PlannedDays,
                        DaysDifference = daysDifference,
                        TimeWorked = worked?.WorkedTime ?? TimeSpan.Zero,
                        TimePlanned = planned?.PlannedTime,
                        TimeDifference = daysDifference.HasValue ? TimeSpan.FromHours(daysDifference.Value * settings.WorkHoursPerWorkday.TotalHours) : null,
                        BudgetWorked = worked?.WorkedBudget ?? 0,
                        BudgetPlanned = planned?.PlannedBudget,
                        BudgetDifference = planned?.PlannedBudget - planned?.WorkedBudget,
                        PlannedStart = plannedTimeSpan?.Start,
                        PlannedEnd = plannedTimeSpan?.End,
                        PlannedIsPartial = plannedTimeSpan != null && !filter.SelectedPeriod.Contains(plannedTimeSpan),
                        Currency = settings.Company.Currency,
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
        var settings = await _settingService.GetSettings(cancellationToken);
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
                        CustomerHidden = withOrder?.CustomerHidden ?? withoutOrder.CustomerHidden,
                        WorkedTime = workTimes.Sum(x => x.WorkedTime),
                        WorkedBudget = workTimes.Sum(x => x.WorkedBudget),
                        PlannedStart = withOrder?.PlannedStart,
                        PlannedEnd = withOrder?.PlannedEnd,
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
        var workedTimesPerCustomer = workedTimesPerOrder
            .GroupBy(x => x.CustomerId)
            .Select(orderWorkItems => new CustomerWorkTime
            {
                CustomerId = orderWorkItems.Key,
                CustomerTitle = orderWorkItems.First().CustomerTitle,
                CustomerHidden = orderWorkItems.First().CustomerHidden,
                WorkedTime = orderWorkItems.Sum(x => x.WorkedTime),
                WorkedBudget = orderWorkItems.Sum(x => x.WorkedBudget),
                PlannedStart = orderWorkItems.Min(x => x.PlannedStart),
                PlannedEnd = orderWorkItems.Max(x => x.PlannedEnd),
            })
            .ToList();

        return workedTimesPerCustomer;
    }

    private async Task<List<CustomerWorkTime>> GetWorkedTimesWithoutOrderPerCustomer(ChartFilter filter, CancellationToken cancellationToken)
    {
        var timeSheetsPerCustomer = await _dbRepository
           .GetGrouped(
               groupBy: timeSheet => new { timeSheet.Customer.Id, timeSheet.Customer.Title, timeSheet.Customer.Hidden },
               select: timeSheets => new
               {
                   CustomerId = timeSheets.Key.Id,
                   CustomerTitle = timeSheets.Key.Title,
                   CustomerHidden = timeSheets.Key.Hidden,
                   WorkedTime = TimeSpan.FromSeconds(timeSheets.Sum(f => (double)f.StartDateLocal.DiffSeconds(f.StartDateOffset, f.EndDateLocal, f.EndDateOffset))),
                   timeSheets.FirstOrDefault().Customer.HourlyRate,
               },
               where: new[] { filter.WorkedTimes.CreateFilter(), x => x.OrderId == null }.CombineWithConditionalAnd(),
               cancellationToken: cancellationToken
           );

        var workedTimesPerCustomer = timeSheetsPerCustomer
            .Select(timeSheet => new CustomerWorkTime
            {
                CustomerId = timeSheet.CustomerId,
                CustomerTitle = timeSheet.CustomerTitle,
                CustomerHidden = timeSheet.CustomerHidden,
                WorkedTime = timeSheet.WorkedTime,
                WorkedBudget = timeSheet.WorkedTime.TotalHours * timeSheet.HourlyRate,
            })
            .ToList();

        return workedTimesPerCustomer;
    }

    private async Task<List<CustomerWorkTime>> GetPlannedTimesPerCustomer(ChartFilter filter, CancellationToken cancellationToken)
    {
        var settings = await _settingService.GetSettings(cancellationToken);
        var workedTimesPerOrder = await _orderChartService.GetWorkedTimesPerOrder(filter, cancellationToken);
        var plannedTimesPerOrder = await _orderChartService.GetPlannedTimesPerOrder(filter, cancellationToken);
        var plannedTimesPerCustomer = plannedTimesPerOrder
            .OuterJoin(
                workedTimesPerOrder,
                planned => planned.OrderId,
                worked => worked.OrderId,
                (planned, worked) => new { Planned = planned, Worked = worked }
            )
            .GroupBy(x => x.Planned.CustomerId)
            .Select(orderWorkItems => new CustomerWorkTime
            {
                CustomerId = orderWorkItems.Key,
                CustomerTitle = orderWorkItems.First().Planned.CustomerTitle,
                CustomerHidden = orderWorkItems.First().Planned.CustomerHidden,
                PlannedTime = orderWorkItems.Sum(x => x.Planned.PlannedTime),
                PlannedBudget = orderWorkItems.Sum(x => x.Planned.PlannedBudget),
                WorkedTime = orderWorkItems.Sum(x => x.Worked?.WorkedTime ?? TimeSpan.Zero),
                WorkedBudget = orderWorkItems.Sum(x => x.Worked?.WorkedBudget ?? 0),
                PlannedStart = orderWorkItems.Min(x => x.Planned.PlannedStart),
                PlannedEnd = orderWorkItems.Max(x => x.Planned.PlannedEnd)
            })
            .ToList();

        foreach (var workTime in plannedTimesPerCustomer)
        {
            workTime.PlannedDays = workTime.PlannedTime.TotalHours / settings.WorkHoursPerWorkday.TotalHours;
            workTime.WorkedDays = workTime.WorkedTime.TotalHours / settings.WorkHoursPerWorkday.TotalHours;
        }

        return plannedTimesPerCustomer;
    }
}