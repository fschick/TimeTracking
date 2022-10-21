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
public class CustomerChartServiceTests
{
    [DataTestMethod]
    [WorkTimesPerCustomerDataSource]
    public async Task WhenGetWorkTimesPerCustomerRequested_ResultMatchesExpectedValues(string testCaseJson)
    {
        // Prepare
        var testCase = TestCase.FromJson<WorkTimesPerCustomerTestCase>(testCaseJson);

        using var autoFake = new AutoFake();
        await autoFake.ConfigureInMemoryDatabase();
        autoFake.Provide(FakeAutoMapper.Mapper);
        autoFake.Provide<IRepository, Repository<TimeTrackingDbContext>>();
        autoFake.Provide<IWorkdayService, WorkdayService>();
        autoFake.Provide<IOrderChartService, OrderChartService>();
        autoFake.Provide<ICustomerChartService, CustomerChartService>();

        var repository = autoFake.Resolve<IRepository>();
        await repository.AddRange(testCase.MasterData);
        await repository.AddRange(testCase.TimeSheets);
        await repository.SaveChanges();

        // Act
        var customerChartService = autoFake.Resolve<ICustomerChartService>();
        var workTimesPerCustomer = await customerChartService.GetWorkTimesPerCustomer(testCase.Filters);

        // Check
        using var _ = new AssertionScope();
        workTimesPerCustomer.Should().HaveSameCount(testCase.Expected);
        workTimesPerCustomer.Should().BeEquivalentTo(testCase.Expected, options => options
            .WithStrictOrdering()
            .Using(new LimitToThreeDigitsComparer())
            .Using(new LimitToSecondsComparer())
        );
    }
}

public class WorkTimesPerCustomerDataSourceAttribute : TestCaseDataSourceAttribute, ITestDataSource
{
    public WorkTimesPerCustomerDataSourceAttribute() : base(GetTestCases()) { }

    private static List<TestCase> GetTestCases()
    {
        var activity = FakeActivity.Create();

        var customer1 = FakeCustomer.Create("1_", 100);
        var order1A = FakeOrder.Create(customer1.Id, FakeDateTime.Offset("2020-01-01"), FakeDateTime.Offset("2020-12-31"), "1000", 100, 1200, "1A_");

        var customer2 = FakeCustomer.Create("2_", 100);
        var order2A = FakeOrder.Create(customer2.Id, FakeDateTime.Offset("2020-07-01"), FakeDateTime.Offset("2020-12-31"), "2100", 050, 0600, "2A_");
        var order2B = FakeOrder.Create(customer2.Id, FakeDateTime.Offset("2021-01-01"), FakeDateTime.Offset("2021-06-30"), "2200", 075, 0600, "2B_");

        var customer3 = FakeCustomer.Create("3_", 0);
        var order3A = FakeOrder.Create(customer3.Id, FakeDateTime.Offset("2021-01-01"), FakeDateTime.Offset("2021-06-30"), "3100", 050, 1200, "3A_");

        var masterData = new List<IIdEntityModel> { activity, customer1, order1A, customer2, order2A, order2B, customer3, order3A };

        return new List<TestCase>
        {
            new WorkTimesPerCustomerTestCase
            {
                Identifier = "When_time_sheet_with_and_without_order_exists_than_worked_is_calculated_from_both_while_planned_and_difference_exclude_time_sheet_without_order",
                MasterData = new List<IIdEntityModel> { activity, customer2, order2A },
                TimeSheets = new List<TimeSheet>
                {
                    CreateTimeSheet(customer2, activity, order2A, "2020-03-01 08:00", "2020-03-01 12:00"),
                    CreateTimeSheet(customer2, activity, null,    "2020-03-01 08:00", "2020-03-01 12:00"),
                },
                Expected = new List<object>
                {
                    new {
                        CustomerTitle = "2_Customer",
                        // 2 * 4 (08:00-12:00) = 8 hours
                        DaysWorked = 1,
                        // 600 order budget / 50 order hourlyRate = 12 hours
                        DaysPlanned = 1.5,
                        // 1.5 planned days - 4 hours (08:00-12:00) from time sheets with order = 8 hours
                        DaysDifference = 1,

                        //  Same calculation as for Days(Worked|Planned|Difference)
                        TimeWorked = TimeSpan.FromHours(8),
                        TimePlanned = TimeSpan.FromHours(12),
                        TimeDifference = TimeSpan.FromHours(8),
                        
                        // 200 (4 * 50) from order budget + 400 (4 * 100) from customer's hourly rate
                        BudgetWorked = 600,
                        // Budget from order
                        BudgetPlanned = 600,
                        // 600 budget from order - 200 from time sheets with order
                        BudgetDifference = 400
                    },
                },
            },
            new WorkTimesPerCustomerTestCase
            {
                Identifier = "When_order_time_range_is_cut_by_selection_values_are_calculated_proportionally",
                MasterData = masterData,
                TimeSheets = new List<TimeSheet>
                {
                    CreateTimeSheet(customer1, activity, order1A, "2020-07-01 08:00", "2020-07-01 16:00"),
                },
                Filters = FakeFilters.Create("<2020-08-01", ">=2020-07-01"),
                Expected = new List<object>
                {
                    new { CustomerTitle = "1_Customer", DaysPlanned = 0.132, DaysDifference = -0.868, TimePlanned = TimeSpan.FromHours(1.05343), PlannedIsPartial = true },
                    new { CustomerTitle = "2_Customer", DaysPlanned = 0.261, DaysDifference = +0.261, TimePlanned = TimeSpan.FromHours(2.09084), PlannedIsPartial = true },
                },
            },
            new WorkTimesPerCustomerTestCase
            {
                Identifier = "Orders_within_selected_time_range_are_shown_regardless_of_existing_time_sheets",
                MasterData = masterData,
                TimeSheets = new List<TimeSheet>(),
                Filters = FakeFilters.Create("<2021-01-01", ">=2020-01-01"),
                Expected = new List<object>
                {
                    new { CustomerTitle = "1_Customer" },
                    new { CustomerTitle = "2_Customer" },
                },
            },
            new WorkTimesPerCustomerTestCase
            {
                Identifier = "Orders_outside_selected_time_range_are_shown_when_time_sheets_exists",
                MasterData = masterData,
                TimeSheets = new List<TimeSheet>
                {
                    CreateTimeSheet(customer1, activity, order1A, "2021-02-01 08:00", "2021-02-01 16:00"),
                },
                Filters = FakeFilters.Create("<2022-01-01", ">=2021-01-01"),
                Expected = new List<object>
                {
                    new { CustomerTitle = "1_Customer", PlannedIsPartial = true },
                    new { CustomerTitle = "2_Customer", PlannedIsPartial = false },
                    new { CustomerTitle = "3_Customer", PlannedIsPartial = false },
                },
            },
            new WorkTimesPerCustomerTestCase
            {
                Identifier = "When_selected_time_range_is_one_day_before_start_day_of_order_than_order_is_excluded",
                MasterData = new List<IIdEntityModel> { activity, customer2, order2B },
                TimeSheets = new List<TimeSheet>(),
                Filters = FakeFilters.Create("<2020-01-01", ">=2020-12-31"),
                Expected = new List<object>(),
            },
            new WorkTimesPerCustomerTestCase
            {
                Identifier = "When_selected_time_range_is_start_day_of_order_than_order_is_found",
                MasterData = new List<IIdEntityModel> { activity, customer2, order2B },
                TimeSheets = new List<TimeSheet>(),
                Filters = FakeFilters.Create("<2021-01-02", ">=2021-01-01"),
                Expected = new List<object>
                {
                    new { CustomerTitle = "2_Customer" },
                },
            },
            new WorkTimesPerCustomerTestCase
            {
                Identifier = "When_selected_time_range_is_due_day_of_order_than_order_is_found",
                MasterData = new List<IIdEntityModel> { activity, customer2, order2B },
                TimeSheets = new List<TimeSheet>(),
                Filters = FakeFilters.Create("<2021-07-01", ">=2021-06-30"),
                Expected = new List<object>
                {
                    new { CustomerTitle = "2_Customer" },
                },
            },
            new WorkTimesPerCustomerTestCase
            {
                Identifier = "When_selected_time_range_is_one_day_after_due_date_of_order_than_order_is_excluded",
                MasterData = new List<IIdEntityModel> { activity, customer2, order2B },
                TimeSheets = new List<TimeSheet>(),
                Filters = FakeFilters.Create("<2021-07-02", ">=2021-07-01"),
                Expected = new List<object>(),
            },
            new WorkTimesPerCustomerTestCase
            {
                Identifier = "When_full_time_range_of_order_is_selected_than_result_match_expected_values",
                MasterData = masterData,
                TimeSheets = new List<TimeSheet>
                {
                    CreateTimeSheet(customer1, activity, order1A, "2020-03-01 08:00", "2020-03-01 16:00"),
                    CreateTimeSheet(customer1, activity, null,    "2020-03-01 08:00", "2020-03-01 16:00"),

                    CreateTimeSheet(customer2, activity, order2A, "2020-03-01 08:00", "2020-03-01 12:00"),
                    CreateTimeSheet(customer2, activity, order2B, "2020-03-01 08:00", "2020-03-01 12:00"),
                    CreateTimeSheet(customer2, activity, null,    "2020-03-01 08:00", "2020-03-01 16:00"),

                    CreateTimeSheet(customer3, activity, order3A, "2020-03-01 08:00", "2020-03-01 16:00"),
                    CreateTimeSheet(customer3, activity, null,    "2020-03-01 08:00", "2020-03-01 16:00"),
                },
                Expected = new List<object>
                {
                    new { CustomerTitle = "1_Customer", DaysWorked = 2, DaysPlanned = 1.5, DaysDifference = 0.5, TimeWorked = TimeSpan.FromHours(16), TimePlanned = TimeSpan.FromHours(12), TimeDifference = TimeSpan.FromHours(04), BudgetWorked = 1600, BudgetPlanned = 1200, BudgetDifference = 400, PlannedIsPartial = false, TotalWorkedPercentage = 0.333, TotalPlannedPercentage = 0.214 },
                    new { CustomerTitle = "2_Customer", DaysWorked = 2, DaysPlanned = 2.5, DaysDifference = 1.5, TimeWorked = TimeSpan.FromHours(16), TimePlanned = TimeSpan.FromHours(20), TimeDifference = TimeSpan.FromHours(12), BudgetWorked = 1300, BudgetPlanned = 1200, BudgetDifference = 700, PlannedIsPartial = false, TotalWorkedPercentage = 0.333, TotalPlannedPercentage = 0.357 },
                    new { CustomerTitle = "3_Customer", DaysWorked = 2, DaysPlanned = 3.0, DaysDifference = 2.0, TimeWorked = TimeSpan.FromHours(16), TimePlanned = TimeSpan.FromHours(24), TimeDifference = TimeSpan.FromHours(16), BudgetWorked = 0400, BudgetPlanned = 1200, BudgetDifference = 800, PlannedIsPartial = false, TotalWorkedPercentage = 0.333, TotalPlannedPercentage = 0.429 },
                },
            },
            new WorkTimesPerCustomerTestCase
            {
                Identifier = "No_orders_and_time_sheets",
                Expected = new List<object>(),
            },
        };
    }

    private static TimeSheet CreateTimeSheet(Customer customer, Activity activity, Order order, string startDate, string endDate)
        => FakeTimeSheet.Create(customer.Id, activity.Id, null, order?.Id, FakeDateTime.Offset(startDate), FakeDateTime.Offset(endDate));
}

public class WorkTimesPerCustomerTestCase : TestCase
{
    public List<IIdEntityModel> MasterData { get; set; } = new();
    public List<TimeSheet> TimeSheets { get; set; } = new();
    public TimeSheetFilterSet Filters { get; set; } = FakeFilters.Empty();
    public List<object> Expected { get; set; } = new();
}