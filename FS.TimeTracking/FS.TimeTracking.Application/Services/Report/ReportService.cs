using FS.FilterExpressionCreator.Enums;
using FS.FilterExpressionCreator.Extensions;
using FS.FilterExpressionCreator.Filters;
using FS.FilterExpressionCreator.Models;
using FS.TimeTracking.Application.Extensions;
using FS.TimeTracking.Shared.DTOs.MasterData;
using FS.TimeTracking.Shared.DTOs.Report;
using FS.TimeTracking.Shared.DTOs.TimeTracking;
using FS.TimeTracking.Shared.Extensions;
using FS.TimeTracking.Shared.Interfaces.Application.Services.MasterData;
using FS.TimeTracking.Shared.Interfaces.Application.Services.Report;
using FS.TimeTracking.Shared.Interfaces.Application.Services.Shared;
using FS.TimeTracking.Shared.Interfaces.Repository.Services;
using FS.TimeTracking.Shared.Models.MasterData;
using FS.TimeTracking.Shared.Models.Report;
using FS.TimeTracking.Shared.Models.TimeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Application.Services.Report;

/// <inheritdoc />
public class ReportService : IReportService
{
    private readonly IRepository _repository;
    private readonly IWorkdayService _workdayService;
    private readonly ISettingService _settingService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReportService" /> class.
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <param name="workdayService">The workday service.</param>
    /// <param name="settingService">The setting service.</param>
    public ReportService(IRepository repository, IWorkdayService workdayService, ISettingService settingService)
    {
        _repository = repository;
        _workdayService = workdayService;
        _settingService = settingService;
    }

    /// <inheritdoc />
    public async Task<List<WorkTimeDto>> GetWorkTimesPerCustomer(EntityFilter<TimeSheetDto> timeSheetFilter, EntityFilter<ProjectDto> projectFilter, EntityFilter<CustomerDto> customerFilter, EntityFilter<ActivityDto> activityFilter, EntityFilter<OrderDto> orderFilter, EntityFilter<HolidayDto> holidayFilter, CancellationToken cancellationToken = default)
    {
        var settings = await _settingService.Get(cancellationToken);
        var filter = CreateFilter(timeSheetFilter, projectFilter, customerFilter, activityFilter, orderFilter, holidayFilter);
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
                    return new WorkTimeDto
                    {
                        Id = worked?.CustomerId ?? planned.CustomerId,
                        CustomerTitle = worked?.CustomerTitle ?? planned?.CustomerTitle ?? throw new InvalidOperationException($"Unable to get {nameof(WorkTimeDto.CustomerTitle)}"),
                        TimeWorked = worked?.WorkedTime ?? TimeSpan.Zero,
                        DaysWorked = worked?.WorkedDays ?? 0,
                        RatioTotalWorked = totalWorkedDays != 0 ? (worked?.WorkedDays ?? 0) / totalWorkedDays : 0,
                        BudgetWorked = worked?.WorkedBudget ?? 0,
                        TimePlanned = planned?.PlannedTime,
                        DaysPlanned = planned?.PlannedDays,
                        RatioTotalPlanned = totalPlannedDays != 0 ? (planned?.PlannedDays ?? 0) / totalPlannedDays : null,
                        BudgetPlanned = planned?.PlannedBudget,
                        PlannedHourlyRate = planned?.HourlyRate,
                        Currency = settings.Currency,
                        PlannedStart = planned?.PlannedStart,
                        PlannedEnd = planned?.PlannedEnd,
                        PlannedIsPartial = plannedTimeSpan != null && !filter.SelectedPeriod.Contains(plannedTimeSpan),
                    };
                })
            .OrderBy(x => x.PlannedStart == null)
            .ThenBy(x => x.PlannedStart)
            .ThenBy(x => x.CustomerTitle)
            .ThenBy(x => x.OrderNumber)
            .ToList();

        return result;
    }

    /// <inheritdoc />
    public async Task<List<WorkTimeDto>> GetWorkTimesPerOrder(EntityFilter<TimeSheetDto> timeSheetFilter, EntityFilter<ProjectDto> projectFilter, EntityFilter<CustomerDto> customerFilter, EntityFilter<ActivityDto> activityFilter, EntityFilter<OrderDto> orderFilter, EntityFilter<HolidayDto> holidayFilter, CancellationToken cancellationToken = default)
    {
        var settings = await _settingService.Get(cancellationToken);
        var filter = CreateFilter(timeSheetFilter, projectFilter, customerFilter, activityFilter, orderFilter, holidayFilter);
        var workedTimesPerOrder = await GetWorkedTimesPerOrder(filter, cancellationToken);
        var plannedTimesPerOrder = await GetPlannedTimesPerOrder(filter, cancellationToken);

        var totalWorkedDays = workedTimesPerOrder.Sum(x => x.WorkedDays);
        var totalPlannedDays = plannedTimesPerOrder.Sum(x => x.PlannedDays);

        return workedTimesPerOrder
            .CrossJoin(
                plannedTimesPerOrder,
                worked => worked.OrderId,
                planned => planned.OrderId,
                (worked, planned) =>
                {
                    if (worked == null && planned == null)
                        throw new InvalidOperationException("Planned and worked entities are null");
                    if (planned == null)
                        throw new InvalidOperationException("Planned entity is null");

                    var plannedTimeSpan = new Section<DateTimeOffset>(planned.PlannedStart, planned.PlannedEnd);
                    return new WorkTimeDto
                    {
                        Id = worked?.OrderId ?? planned.OrderId,
                        OrderTitle = worked?.OrderTitle ?? planned.OrderTitle,
                        OrderNumber = worked?.OrderNumber ?? planned.OrderNumber,
                        CustomerTitle = worked?.CustomerTitle ?? planned.CustomerTitle,
                        TimeWorked = worked?.WorkedTime ?? TimeSpan.Zero,
                        DaysWorked = worked?.WorkedDays ?? 0,
                        RatioTotalWorked = totalWorkedDays != 0 ? (worked?.WorkedDays ?? 0) / totalWorkedDays : 0,
                        BudgetWorked = worked?.WorkedBudget ?? 0,
                        TimePlanned = planned.PlannedTime,
                        DaysPlanned = planned.PlannedDays,
                        RatioTotalPlanned = totalPlannedDays != 0 ? planned.PlannedDays / totalPlannedDays : 0,
                        BudgetPlanned = planned.PlannedBudget,
                        PlannedStart = planned.PlannedStart,
                        PlannedEnd = planned.PlannedEnd,
                        PlannedIsPartial = !filter.SelectedPeriod.Contains(plannedTimeSpan),
                        PlannedHourlyRate = planned.HourlyRate,
                        Currency = settings.Currency,
                    };
                })
            .OrderBy(x => x.PlannedStart)
            .ThenBy(x => x.CustomerTitle)
            .ThenBy(x => x.OrderNumber)
            .ToList();
    }

    private async Task<List<CustomerWorkTime>> GetWorkedTimesPerCustomer(Filter filter, CancellationToken cancellationToken)
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
                        HourlyRate = GetAverageHourlyRate(workTimes, x => x?.WorkedTime),
                    };
                }
            )
            .ToList();

        foreach (var workTime in workedTimesPerCustomer)
            workTime.WorkedDays = workTime.WorkedTime.TotalHours / settings.WorkHoursPerWorkday.TotalHours;

        return workedTimesPerCustomer;
    }

    private async Task<List<CustomerWorkTime>> GetWorkedTimesWithOrderPerCustomer(Filter filter, CancellationToken cancellationToken)
    {
        var workedTimesPerOrder = await GetWorkedTimesPerOrder(filter, cancellationToken);
        var workTimesWithOrder = workedTimesPerOrder
            .GroupBy(x => x.CustomerId)
            .Select(orderWorkItems => new CustomerWorkTime
            {
                CustomerId = orderWorkItems.Key,
                CustomerTitle = orderWorkItems.First().CustomerTitle,
                WorkedTime = orderWorkItems.Sum(x => x.WorkedTime),
                HourlyRate = GetAverageHourlyRate(orderWorkItems.ToList(), x => x?.WorkedTime)
            })
            .ToList();
        return workTimesWithOrder;
    }

    private async Task<List<CustomerWorkTime>> GetWorkedTimesWithoutOrderPerCustomer(Filter filter, CancellationToken cancellationToken)
        => await _repository
            .GetGrouped(
                groupBy: x => new { x.Project.Customer.Id, x.Project.Customer.Title },
                select: x => new CustomerWorkTime
                {
                    CustomerId = x.Key.Id,
                    CustomerTitle = x.Key.Title,
                    WorkedTime = TimeSpan.FromSeconds(x.Sum(f => (double)f.StartDateLocal.DiffSeconds(f.StartDateOffset, f.EndDateLocal))),
                    HourlyRate = x.Min(t => t.Project.Customer.HourlyRate),
                },
                where: new[] { filter.WorkedTimes.CreateFilter(), x => x.OrderId == null }.CombineWithConditionalAnd(),
                cancellationToken: cancellationToken
            );

    private async Task<List<CustomerWorkTime>> GetPlannedTimesPerCustomer(Filter filter, CancellationToken cancellationToken)
    {
        var settings = await _settingService.Get(cancellationToken);

        var plannedTimesPerOrder = await GetPlannedTimesPerOrder(filter, cancellationToken);
        var plannedTimesPerCustomer = plannedTimesPerOrder
            .GroupBy(x => x.CustomerId)
            .Select(orderWorkItems => new CustomerWorkTime
            {
                CustomerId = orderWorkItems.Key,
                CustomerTitle = orderWorkItems.First().CustomerTitle,
                PlannedTime = orderWorkItems.Sum(x => x.PlannedTime),
                HourlyRate = GetAverageHourlyRate(orderWorkItems.ToList(), x => x?.PlannedTime),
                PlannedStart = orderWorkItems.Min(x => x.PlannedStart),
                PlannedEnd = orderWorkItems.Max(x => x.PlannedEnd)
            })
            .ToList();

        foreach (var workTime in plannedTimesPerCustomer)
            workTime.PlannedDays = workTime.PlannedTime.TotalHours / settings.WorkHoursPerWorkday.TotalHours;

        return plannedTimesPerCustomer;
    }

    private static double GetAverageHourlyRate(IReadOnlyCollection<WorkTime> workTimes, Func<WorkTime, TimeSpan?> timeSelector)
    {
        var times = workTimes.Select(x => new { Hours = timeSelector(x) ?? TimeSpan.Zero, x.HourlyRate }).ToList();
        var hours = times.Sum(o => o.Hours.TotalHours);
        var budget = times.Sum(o => o.Hours.TotalHours * o.HourlyRate);
        var averageHourlyRate = hours != 0 ? budget / hours : 0;
        return averageHourlyRate;
    }

    private async Task<List<OrderWorkTime>> GetWorkedTimesPerOrder(Filter filter, CancellationToken cancellationToken)
    {
        var settings = await _settingService.Get(cancellationToken);

        var workedTimesPerOrder = await _repository
            .GetGrouped(
                groupBy: x => new { x.OrderId, x.Order.Title, x.Order.Number },
                select: x => new OrderWorkTime
                {
                    OrderId = x.Key.OrderId.Value,
                    OrderTitle = x.Key.Title,
                    OrderNumber = x.Key.Number,
                    WorkedTime = TimeSpan.FromSeconds(x.Sum(f => (double)f.StartDateLocal.DiffSeconds(f.StartDateOffset, f.EndDateLocal))),
                    HourlyRate = x.FirstOrDefault().Order.HourlyRate,
                    CustomerId = x.FirstOrDefault().Project.Customer.Id,
                    CustomerTitle = x.FirstOrDefault().Project.Customer.Title,
                },
                where: new[] { filter.WorkedTimes.CreateFilter(), x => x.OrderId != null }.CombineWithConditionalAnd(),
                cancellationToken: cancellationToken
            );

        foreach (var workTime in workedTimesPerOrder)
            workTime.WorkedDays = workTime.WorkedTime.TotalHours / settings.WorkHoursPerWorkday.TotalHours;

        return workedTimesPerOrder;
    }

    private async Task<List<OrderWorkTime>> GetPlannedTimesPerOrder(Filter filter, CancellationToken cancellationToken)
    {
        var settings = await _settingService.Get(cancellationToken);

        var orders = await _repository
            .Get(
                select: (Order x) => x,
                where: filter.PlannedTimes,
                includes: new[] { nameof(Order.Customer) },
                cancellationToken: cancellationToken
            );

        var plannedTimesPerOrder = await orders
            .SelectAsync(async order =>
            {
                var plannedTime = await GetPlannedTimeForPeriod(order, filter.SelectedPeriod);
                return new OrderWorkTime
                {
                    OrderId = order.Id,
                    OrderTitle = order.Title,
                    OrderNumber = order.Number,
                    CustomerId = order.CustomerId,
                    CustomerTitle = order.Customer.Title,
                    PlannedTime = plannedTime,
                    PlannedDays = plannedTime.TotalHours / settings.WorkHoursPerWorkday.TotalHours,
                    HourlyRate = order.HourlyRate,
                    PlannedStart = order.StartDate,
                    PlannedEnd = order.DueDate,
                };
            })
            .ToListAsync();

        return plannedTimesPerOrder;
    }

    private async Task<TimeSpan> GetPlannedTimeForPeriod(Order order, Section<DateTimeOffset> selectedPeriod)
    {
        var orderPeriod = new Section<DateTimeOffset>(order.StartDate, order.DueDate.AddDays(1));
        var orderWorkdays = await _workdayService.GetWorkdays(orderPeriod);
        var orderWorkHours = order.HourlyRate != 0 ? order.Budget / order.HourlyRate : 0;

        var planningPeriod = orderPeriod.Intersection(selectedPeriod);
        var plannedWorkDays = await _workdayService.GetWorkdays(planningPeriod) ?? orderWorkdays;

        var ratio = orderWorkdays.PersonalWorkdays.Count != 0
            ? plannedWorkDays.PersonalWorkdays.Count / (double)orderWorkdays.PersonalWorkdays.Count
            : 1;
        return TimeSpan.FromHours(orderWorkHours * ratio);
    }

    private static Filter CreateFilter(EntityFilter<TimeSheetDto> timeSheetFilter, EntityFilter<ProjectDto> projectFilter, EntityFilter<CustomerDto> customerFilter, EntityFilter<ActivityDto> activityFilter, EntityFilter<OrderDto> orderFilter, EntityFilter<HolidayDto> holidayFilter)
    {
        var workedTimesFilter = FilterExtensions.CreateTimeSheetFilter(timeSheetFilter, projectFilter, customerFilter, activityFilter, orderFilter, holidayFilter);
        var plannedTimesFilter = FilterExtensions.CreateOrderFilter(timeSheetFilter, projectFilter, customerFilter, activityFilter, orderFilter, holidayFilter);
        var selectedPeriod = FilterExtensions.GetSelectedPeriod(timeSheetFilter);

        plannedTimesFilter = plannedTimesFilter
            .Replace(x => x.DueDate, FilterOperator.GreaterThanOrEqual, selectedPeriod.Start)
            .Replace(x => x.StartDate, FilterOperator.LessThan, selectedPeriod.End);

        return new Filter(workedTimesFilter, plannedTimesFilter, selectedPeriod);
    }

    private record struct Filter(EntityFilter<TimeSheet> WorkedTimes, EntityFilter<Order> PlannedTimes, Section<DateTimeOffset> SelectedPeriod)
    {
        public readonly Section<DateTimeOffset> SelectedPeriod = SelectedPeriod;
        public readonly EntityFilter<TimeSheet> WorkedTimes = WorkedTimes;
        public readonly EntityFilter<Order> PlannedTimes = PlannedTimes;
    }
}
