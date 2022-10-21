using Autofac.Extras.FakeItEasy;
using FluentAssertions;
using FluentAssertions.Execution;
using FS.TimeTracking.Application.Services.Chart;
using FS.TimeTracking.Application.Services.Shared;
using FS.TimeTracking.Application.Tests.Attributes;
using FS.TimeTracking.Application.Tests.Extensions;
using FS.TimeTracking.Application.Tests.Models;
using FS.TimeTracking.Application.Tests.Services;
using FS.TimeTracking.Core.Interfaces.Application.Services.Chart;
using FS.TimeTracking.Core.Interfaces.Application.Services.Shared;
using FS.TimeTracking.Core.Interfaces.Models;
using FS.TimeTracking.Core.Interfaces.Repository.Services;
using FS.TimeTracking.Core.Models.Application.MasterData;
using FS.TimeTracking.Core.Models.Application.TimeTracking;
using FS.TimeTracking.Core.Models.Filter;
using FS.TimeTracking.Repository.DbContexts;
using FS.TimeTracking.Repository.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace FS.TimeTracking.Application.Tests.Tests.Services;

[TestClass, ExcludeFromCodeCoverage]
public class OrderChartServiceTests
{
    [DataTestMethod]
    [WorkTimesPerOrderDataSource]
    public async Task WhenGetWorkTimesPerOrderRequested_ResultMatchesExpectedValues(string testCaseJson)
    {
        // Prepare
        var testCase = TestCase.FromJson<WorkTimesPerOrderTestCase>(testCaseJson);

        using var autoFake = new AutoFake();
        await autoFake.ConfigureInMemoryDatabase();
        autoFake.Provide(FakeAutoMapper.Mapper);
        autoFake.Provide<IRepository, Repository<TimeTrackingDbContext>>();
        autoFake.Provide<IWorkdayService, WorkdayService>();
        autoFake.Provide<IOrderChartService, OrderChartService>();

        var repository = autoFake.Resolve<IRepository>();
        await repository.AddRange(testCase.MasterData);
        await repository.AddRange(testCase.TimeSheets);
        await repository.SaveChanges();

        // Act
        var orderChartService = autoFake.Resolve<IOrderChartService>();
        var workTimesPerOrder = await orderChartService.GetWorkTimesPerOrder(testCase.Filters);

        // Check
        using var _ = new AssertionScope();
        workTimesPerOrder.Should().HaveSameCount(testCase.Expected);
        workTimesPerOrder.Should().BeEquivalentTo(testCase.Expected, options => options
              .WithStrictOrdering()
              .Using(new LimitToThreeDigitsComparer())
              .Using(new LimitToSecondsComparer())
        );
    }
}

public class WorkTimesPerOrderDataSourceAttribute : TestCaseDataSourceAttribute
{
    public WorkTimesPerOrderDataSourceAttribute() : base(GetTestCases()) { }

    private static List<TestCase> GetTestCases()
    {
        var activity = FakeActivity.Create();

        var customer1 = FakeCustomer.Create("1_");
        var order1A = FakeOrder.Create(customer1.Id, FakeDateTime.Offset("2020-01-01"), FakeDateTime.Offset("2020-12-31"), "1100", 100, 1200, "1A_");

        var customer2 = FakeCustomer.Create("2_");
        var order2A = FakeOrder.Create(customer2.Id, FakeDateTime.Offset("2020-07-01"), FakeDateTime.Offset("2020-12-31"), "2100", 50, 600, "2A_");
        var order2B = FakeOrder.Create(customer2.Id, FakeDateTime.Offset("2021-01-01"), FakeDateTime.Offset("2021-06-30"), "2200", 75, 600, "2B_");

        var masterData = new List<IIdEntityModel> { activity, customer1, order1A, customer2, order2A, order2B };

        return new List<TestCase>
        {
            new WorkTimesPerOrderTestCase
            {
                Identifier = "WhenOrderTimeRangeIsCutBySelectionValuesAreCalculatedProportionally",
                MasterData = masterData,
                TimeSheets = new List<TimeSheet>
                {
                    CreateTimeSheet(customer1, activity, order1A, "2020-07-01 08:00", "2020-07-01 16:00"),
                },
                Filters = FakeFilters.Create("<2020-08-01", ">=2020-07-01"),
                Expected = new List<object>
                {
                    new { OrderTitle = "1A_Order", DaysPlanned = 0.132, DaysDifference = -0.868, TimePlanned = TimeSpan.FromHours(1.05343), PlannedIsPartial = true },
                    new { OrderTitle = "2A_Order", DaysPlanned = 0.261, DaysDifference = +0.261, TimePlanned = TimeSpan.FromHours(2.09084), PlannedIsPartial = true },
                },
            },
            new WorkTimesPerOrderTestCase
            {
                Identifier = "OrdersWithinSelectedTimeRangeAreShownRegardlessOfExistingTimeSheets",
                MasterData = masterData,
                TimeSheets = new List<TimeSheet>(),
                Filters = FakeFilters.Create("<2021-01-01", ">=2020-01-01"),
                Expected = new List<object>
                {
                    new { OrderTitle = "1A_Order", CustomerTitle = "1_Customer" },
                    new { OrderTitle = "2A_Order", CustomerTitle = "2_Customer" },
                },
            },
            new WorkTimesPerOrderTestCase
            {
                Identifier = "OrdersOutsideSelectedTimeRangeAreShownWhenTimeSheetsExists",
                MasterData = masterData,
                TimeSheets = new List<TimeSheet>
                {
                    CreateTimeSheet(customer1, activity, order1A, "2021-02-01 08:00", "2021-02-01 16:00"),
                },
                Filters = FakeFilters.Create("<2022-01-01", ">=2021-01-01"),
                Expected = new List<object>
                {
                    new { OrderTitle = "1A_Order", CustomerTitle = "1_Customer", PlannedIsPartial = true },
                    new { OrderTitle = "2B_Order", CustomerTitle = "2_Customer", PlannedIsPartial = false },
                },
            },
            new WorkTimesPerOrderTestCase
            {
                Identifier = "WhenSelectedTimeRangeIsOneDayBeforeStartDayOfOrderThanOrderIsExcluded",
                MasterData = new List<IIdEntityModel> { activity, customer2, order2B },
                TimeSheets = new List<TimeSheet>(),
                Filters = FakeFilters.Create("<2020-01-01", ">=2020-12-31"),
                Expected = new List<object>(),
            },
            new WorkTimesPerOrderTestCase
            {
                Identifier = "WhenSelectedTimeRangeIsStartDayOfOrderThanOrderIsFound",
                MasterData = new List<IIdEntityModel> { activity, customer2, order2B },
                TimeSheets = new List<TimeSheet>(),
                Filters = FakeFilters.Create("<2021-01-02", ">=2021-01-01"),
                Expected = new List<object>
                {
                    new { OrderTitle = "2B_Order" },
                },
            },
            new WorkTimesPerOrderTestCase
            {
                Identifier = "WhenSelectedTimeRangeIsDueDayOfOrderThanOrderIsFound",
                MasterData = new List<IIdEntityModel> { activity, customer2, order2B },
                TimeSheets = new List<TimeSheet>(),
                Filters = FakeFilters.Create("<2021-07-01", ">=2021-06-30"),
                Expected = new List<object>
                {
                    new { OrderTitle = "2B_Order" },
                },
            },
            new WorkTimesPerOrderTestCase
            {
                Identifier = "WhenSelectedTimeRangeIsOneDayAfterDueDateOfOrderThanOrderIsExcluded",
                MasterData = new List<IIdEntityModel> { activity, customer2, order2B },
                TimeSheets = new List<TimeSheet>(),
                Filters = FakeFilters.Create("<2021-07-02", ">=2021-07-01"),
                Expected = new List<object>(),
            },
            new WorkTimesPerOrderTestCase
            {
                Identifier = "WhenFullTimeRangeOfOrderIsSelectedThanResultMatchExpectedValues",
                MasterData = masterData,
                TimeSheets = new List<TimeSheet>
                {
                    CreateTimeSheet(customer1, activity, order1A, "2020-03-01 08:00", "2020-03-01 16:00"),
                    CreateTimeSheet(customer2, activity, order2A, "2020-03-01 08:00", "2020-03-01 12:00"),
                    CreateTimeSheet(customer2, activity, order2B, "2020-03-01 08:00", "2020-03-01 12:00"),
                },
                Expected = new List<object>
                {
                    new { OrderTitle = "1A_Order", CustomerTitle = "1_Customer", DaysWorked = 1.0, DaysPlanned = 1.5, DaysDifference = 0.5, TimeWorked = TimeSpan.FromHours(8), TimePlanned = TimeSpan.FromHours(12), TimeDifference = TimeSpan.FromHours(4), BudgetWorked = 800, BudgetPlanned = 1200, BudgetDifference = 400, TotalWorkedPercentage = 0.50, TotalPlannedPercentage = 0.375 },
                    new { OrderTitle = "2A_Order", CustomerTitle = "2_Customer", DaysWorked = 0.5, DaysPlanned = 1.5, DaysDifference = 1.0, TimeWorked = TimeSpan.FromHours(4), TimePlanned = TimeSpan.FromHours(12), TimeDifference = TimeSpan.FromHours(8), BudgetWorked = 200, BudgetPlanned = 0600, BudgetDifference = 400, TotalWorkedPercentage = 0.25, TotalPlannedPercentage = 0.375 },
                    new { OrderTitle = "2B_Order", CustomerTitle = "2_Customer", DaysWorked = 0.5, DaysPlanned = 1.0, DaysDifference = 0.5, TimeWorked = TimeSpan.FromHours(4), TimePlanned = TimeSpan.FromHours(08), TimeDifference = TimeSpan.FromHours(4), BudgetWorked = 300, BudgetPlanned = 0600, BudgetDifference = 300, TotalWorkedPercentage = 0.25, TotalPlannedPercentage = 0.250 },
                },
            },
            new WorkTimesPerOrderTestCase
            {
                Identifier = "NoOrdersAndTimeSheets",
                Expected = new List<object>(),
            },
        };
    }

    private static TimeSheet CreateTimeSheet(Customer customer, Activity activity, Order order, string startDate, string endDate)
        => FakeTimeSheet.Create(customer.Id, activity.Id, null, order.Id, FakeDateTime.Offset(startDate), FakeDateTime.Offset(endDate));
}

public class WorkTimesPerOrderTestCase : TestCase
{
    public List<IIdEntityModel> MasterData { get; set; } = new();
    public List<TimeSheet> TimeSheets { get; set; } = new();
    public TimeSheetFilterSet Filters { get; set; } = FakeFilters.Empty();
    public List<object> Expected { get; set; } = new();
}