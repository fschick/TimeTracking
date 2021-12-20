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
        var filter = CreateFilter(timeSheetFilter, projectFilter, customerFilter, activityFilter, orderFilter, holidayFilter);
        var workedTimesPerCustomer = await GetWorkedTimesPerCustomer(filter, cancellationToken);
        var plannedTimesPerCustomer = await GetPlannedTimesPerCustomer(filter, cancellationToken);

        return workedTimesPerCustomer
            .CrossJoin(
                plannedTimesPerCustomer,
                worked => worked.Id,
                planned => planned.Id,
                (worked, planned) => new WorkTimeDto
                {
                    Id = worked?.Id ?? planned?.Id ?? throw new InvalidOperationException(),
                    CustomerTitle = worked?.Title ?? planned?.Title ?? throw new InvalidOperationException(),
                    TimeWorked = worked?.WorkedTime ?? TimeSpan.Zero,
                    DaysWorked = worked?.WorkedDays ?? 0,
                    TimePlanned = planned?.PlannedTime ?? TimeSpan.Zero,
                    DaysPlanned = planned?.PlannedDays ?? 0
                }
            )
            .OrderBy(x => x.CustomerTitle)
            .ToList();
    }

    /// <inheritdoc />
    public async Task<List<WorkTimeDto>> GetWorkTimesPerOrder(EntityFilter<TimeSheetDto> timeSheetFilter, EntityFilter<ProjectDto> projectFilter, EntityFilter<CustomerDto> customerFilter, EntityFilter<ActivityDto> activityFilter, EntityFilter<OrderDto> orderFilter, EntityFilter<HolidayDto> holidayFilter, CancellationToken cancellationToken = default)
    {
        var filter = CreateFilter(timeSheetFilter, projectFilter, customerFilter, activityFilter, orderFilter, holidayFilter);
        var workedTimesPerOrder = await GetWorkedTimesPerOrder(filter, cancellationToken);
        var plannedTimesPerOrder = await GetPlannedTimesPerOrder(filter, cancellationToken);

        var totalWorkedDays = workedTimesPerOrder.Sum(x => x.WorkedDays);
        var totalPlannedDays = plannedTimesPerOrder.Sum(x => x.PlannedDays);

        return workedTimesPerOrder
            .CrossJoin(
                plannedTimesPerOrder,
                worked => worked.Id,
                planned => planned.Id,
                (worked, planned) =>
                {
                    if (worked == null && planned == null)
                        throw new InvalidOperationException("Planned and worked entities are null");
                    if (planned == null)
                        throw new InvalidOperationException("Planned entity is null");

                    var plannedTimeSpan = new Section<DateTimeOffset>(planned.PlannedStart, planned.PlannedEnd);
                    return new WorkTimeDto
                    {
                        Id = worked?.Id ?? planned.Id,
                        OrderTitle = worked?.Title ?? planned.Title,
                        OrderNumber = worked?.Number ?? planned.Number,
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
                        // ReSharper disable once PossiblyImpureMethodCallOnReadonlyVariable
                        PlannedIsPartial = !filter.SelectedPeriod.Contains(plannedTimeSpan),
                        HourlyRate = planned.HourlyRate,
                        Currency = planned.Currency,
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

        var workedTimesPerCustomer = await _repository
            .GetGrouped(
                groupBy: (TimeSheet x) => new { x.Project.CustomerId, x.Project.Customer.Title },
                select: x => new CustomerWorkTime
                {
                    Id = x.Key.CustomerId,
                    Title = x.Key.Title,
                    WorkedTime = TimeSpan.FromSeconds(x.Sum(f => (double)f.StartDateLocal.DiffSeconds(f.StartDateOffset, f.EndDateLocal)))
                },
                where: filter.WorkedTimes,
                cancellationToken: cancellationToken
            );

        foreach (var customerWorkTime in workedTimesPerCustomer)
            customerWorkTime.WorkedDays = customerWorkTime.WorkedTime.TotalHours / settings.WorkHoursPerWorkday.TotalHours;

        return workedTimesPerCustomer;
    }

    private async Task<List<CustomerWorkTime>> GetPlannedTimesPerCustomer(Filter filter, CancellationToken cancellationToken)
    {
        var settings = await _settingService.Get(cancellationToken);

        var plannedTimesPerOrder = await GetPlannedTimesPerOrder(filter, cancellationToken);

        var plannedTimesPerCustomer = plannedTimesPerOrder
            .GroupBy(order => new { order.CustomerId, order.CustomerTitle })
            .Select(orderGroup =>
            {
                var plannedTime = orderGroup.Sum(order => order.PlannedTime);
                return new CustomerWorkTime
                {
                    Id = orderGroup.Key.CustomerId,
                    Title = orderGroup.Key.CustomerTitle,
                    PlannedTime = plannedTime,
                    PlannedDays = plannedTime.TotalHours / settings.WorkHoursPerWorkday.TotalHours,
                };
            })
            .ToList();

        return plannedTimesPerCustomer;
    }

    private async Task<List<OrderWorkTime>> GetWorkedTimesPerOrder(Filter filter, CancellationToken cancellationToken)
    {
        var settings = await _settingService.Get(cancellationToken);

        var workedTimesPerOrder = await _repository
            .GetGrouped(
                groupBy: x => new { x.OrderId, x.Order.Title, x.Order.Number },
                select: x => new OrderWorkTime
                {
                    Id = x.Key.OrderId.Value,
                    Title = x.Key.Title,
                    Number = x.Key.Number,
                    WorkedTime = TimeSpan.FromSeconds(x.Sum(f => (double)f.StartDateLocal.DiffSeconds(f.StartDateOffset, f.EndDateLocal))),
                    HourlyRate = x.Min(f => f.Order.HourlyRate),
                    CustomerTitle = x.Min(f => f.Project.Customer.Title),
                },
                where: new[] { filter.WorkedTimes.CreateFilter(), x => x.Billable && x.OrderId != null }.CombineWithConditionalAnd(),
                cancellationToken: cancellationToken
            );

        foreach (var order in workedTimesPerOrder)
            order.WorkedDays = order.WorkedTime.TotalHours / settings.WorkHoursPerWorkday.TotalHours;

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
                    Id = order.Id,
                    Title = order.Title,
                    Number = order.Number,
                    CustomerId = order.CustomerId,
                    CustomerTitle = order.Customer.Title,
                    PlannedTime = plannedTime,
                    PlannedDays = plannedTime.TotalHours / settings.WorkHoursPerWorkday.TotalHours,
                    HourlyRate = order.HourlyRate,
                    PlannedStart = order.StartDate,
                    PlannedEnd = order.DueDate,
                    Currency = settings.Currency,
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
