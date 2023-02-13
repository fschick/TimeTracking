using FluentAssertions;
using FluentAssertions.Execution;
using FS.TimeTracking.Application.Services.Chart;
using FS.TimeTracking.Application.Services.Shared;
using FS.TimeTracking.Application.Tests.Attributes;
using FS.TimeTracking.Application.Tests.Extensions;
using FS.TimeTracking.Application.Tests.Models;
using FS.TimeTracking.Application.Tests.Services;
using FS.TimeTracking.Application.Tests.Services.FakeModels;
using FS.TimeTracking.Core.Interfaces.Application.Services.Chart;
using FS.TimeTracking.Core.Interfaces.Application.Services.Shared;
using FS.TimeTracking.Core.Interfaces.Models;
using FS.TimeTracking.Core.Interfaces.Repository.Services.Database;
using FS.TimeTracking.Core.Models.Application.MasterData;
using FS.TimeTracking.Core.Models.Application.TimeTracking;
using FS.TimeTracking.Core.Models.Filter;
using Microsoft.Extensions.DependencyInjection;
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

        using var faker = new Faker();
        faker.ConfigureInMemoryDatabase();
        faker.ConfigureAuthorization(false);
        faker.Provide<IWorkdayService, WorkdayService>();
        faker.Provide<IOrderChartService, OrderChartService>();
        faker.Provide<ICustomerChartApiService, CustomerChartService>();

        var dbRepository = faker.GetRequiredService<IDbRepository>();
        await dbRepository.AddRange(testCase.MasterData);
        await dbRepository.AddRange(testCase.TimeSheets);
        await dbRepository.SaveChanges();

        // Act
        var customerChartService = faker.GetRequiredService<ICustomerChartApiService>();
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

public class WorkTimesPerCustomerDataSourceAttribute : TestCaseDataSourceAttribute
{
    public WorkTimesPerCustomerDataSourceAttribute() : base(GetTestCases()) { }

    private static List<TestCase> GetTestCases()
    {
        using var faker = new Faker();

        var activity = faker.Activity.Create();

        var customer1 = faker.Customer.Create("1_", 100);
        var order1A = faker.Order.Create(customer1.Id, faker.DateTime.Offset("2020-01-01"), faker.DateTime.Offset("2020-12-31"), "1000", 100, 1200, "1A_");

        var customer2 = faker.Customer.Create("2_", 100);
        var order2A = faker.Order.Create(customer2.Id, faker.DateTime.Offset("2020-07-01"), faker.DateTime.Offset("2020-12-31"), "2100", 050, 0600, "2A_");
        var order2B = faker.Order.Create(customer2.Id, faker.DateTime.Offset("2021-01-01"), faker.DateTime.Offset("2021-06-30"), "2200", 075, 0600, "2B_");

        var customer3 = faker.Customer.Create("3_", 0);
        var order3A = faker.Order.Create(customer3.Id, faker.DateTime.Offset("2021-01-01"), faker.DateTime.Offset("2021-06-30"), "3100", 050, 1200, "3A_");

        var masterData = new List<IIdEntityModel> { activity, customer1, order1A, customer2, order2A, order2B, customer3, order3A };

        var testCases = new List<TestCase>
        {
            new WorkTimesPerCustomerTestCase
            {
                Identifier = "WhenTimeSheetWithAndWithoutOrderExistsThanWorkedIsCalculatedFromBothWhilePlannedAndDifferenceExcludeTimeSheetWithoutOrder",
                MasterData = new List<IIdEntityModel> { activity, customer2, order2A },
                TimeSheets = new List<TimeSheet>
                {
                    CreateTimeSheet("2020-03-01 08:00", "2020-03-01 12:00", customer2, activity, order2A, faker),
                    CreateTimeSheet("2020-03-01 08:00", "2020-03-01 12:00", customer2, activity,    null, faker),
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
                Identifier = "WhenOrderTimeRangeIsCutBySelectionValuesAreCalculatedProportionally",
                MasterData = masterData,
                TimeSheets = new List<TimeSheet>
                {
                    CreateTimeSheet("2020-07-01 08:00", "2020-07-01 16:00", customer1, activity, order1A, faker),
                },
                Filters = faker.Filters.Create("<2020-08-01", ">=2020-07-01"),
                Expected = new List<object>
                {
                    new { CustomerTitle = "1_Customer", DaysPlanned = 0.132, DaysDifference = -0.868, TimePlanned = TimeSpan.FromHours(1.05343), PlannedIsPartial = true },
                    new { CustomerTitle = "2_Customer", DaysPlanned = 0.261, DaysDifference = +0.261, TimePlanned = TimeSpan.FromHours(2.09084), PlannedIsPartial = true },
                },
            },
            new WorkTimesPerCustomerTestCase
            {
                Identifier = "OrdersWithinSelectedTimeRangeAreShownRegardlessOfExistingTimeSheets",
                MasterData = masterData,
                TimeSheets = new List<TimeSheet>(),
                Filters = faker.Filters.Create("<2021-01-01", ">=2020-01-01"),
                Expected = new List<object>
                {
                    new { CustomerTitle = "1_Customer" },
                    new { CustomerTitle = "2_Customer" },
                },
            },
            new WorkTimesPerCustomerTestCase
            {
                Identifier = "OrdersOutsideSelectedTimeRangeAreShownWhenTimeSheetsExists",
                MasterData = masterData,
                TimeSheets = new List<TimeSheet>
                {
                    CreateTimeSheet("2021-02-01 08:00", "2021-02-01 16:00", customer1, activity, order1A, faker),
                },
                Filters = faker.Filters.Create("<2022-01-01", ">=2021-01-01"),
                Expected = new List<object>
                {
                    new { CustomerTitle = "1_Customer", PlannedIsPartial = true },
                    new { CustomerTitle = "2_Customer", PlannedIsPartial = false },
                    new { CustomerTitle = "3_Customer", PlannedIsPartial = false },
                },
            },
            new WorkTimesPerCustomerTestCase
            {
                Identifier = "WhenSelectedTimeRangeIsOneDayBeforeStartDayOfOrderThanOrderIsExcluded",
                MasterData = new List<IIdEntityModel> { activity, customer2, order2B },
                TimeSheets = new List<TimeSheet>(),
                Filters = faker.Filters.Create("<2020-01-01", ">=2020-12-31"),
                Expected = new List<object>(),
            },
            new WorkTimesPerCustomerTestCase
            {
                Identifier = "WhenSelectedTimeRangeIsStartDayOfOrderThanOrderIsFound",
                MasterData = new List<IIdEntityModel> { activity, customer2, order2B },
                TimeSheets = new List<TimeSheet>(),
                Filters = faker.Filters.Create("<2021-01-02", ">=2021-01-01"),
                Expected = new List<object>
                {
                    new { CustomerTitle = "2_Customer" },
                },
            },
            new WorkTimesPerCustomerTestCase
            {
                Identifier = "WhenSelectedTimeRangeIsDueDayOfOrderThanOrderIsFound",
                MasterData = new List<IIdEntityModel> { activity, customer2, order2B },
                TimeSheets = new List<TimeSheet>(),
                Filters = faker.Filters.Create("<2021-07-01", ">=2021-06-30"),
                Expected = new List<object>
                {
                    new { CustomerTitle = "2_Customer" },
                },
            },
            new WorkTimesPerCustomerTestCase
            {
                Identifier = "WhenSelectedTimeRangeIsOneDayAfterDueDateOfOrderThanOrderIsExcluded",
                MasterData = new List<IIdEntityModel> { activity, customer2, order2B },
                TimeSheets = new List<TimeSheet>(),
                Filters = faker.Filters.Create("<2021-07-02", ">=2021-07-01"),
                Expected = new List<object>(),
            },
            new WorkTimesPerCustomerTestCase
            {
                Identifier = "WhenFullTimeRangeOfOrderIsSelectedThanResultMatchExpectedValues",
                MasterData = masterData,
                TimeSheets = new List<TimeSheet>
                {
                    CreateTimeSheet("2020-03-01 08:00", "2020-03-01 16:00", customer1, activity, order1A, faker),
                    CreateTimeSheet("2020-03-01 08:00", "2020-03-01 16:00", customer1, activity,    null, faker),

                    CreateTimeSheet("2020-03-01 08:00", "2020-03-01 12:00", customer2, activity, order2A, faker),
                    CreateTimeSheet("2020-03-01 08:00", "2020-03-01 12:00", customer2, activity, order2B, faker),
                    CreateTimeSheet("2020-03-01 08:00", "2020-03-01 16:00", customer2, activity,    null, faker),

                    CreateTimeSheet("2020-03-01 08:00", "2020-03-01 16:00", customer3, activity, order3A, faker),
                    CreateTimeSheet("2020-03-01 08:00", "2020-03-01 16:00", customer3, activity,    null, faker),
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
                Identifier = "NoOrdersAndTimeSheets",
                Expected = new List<object>(),
            },
        };

        return testCases;
    }

    private static TimeSheet CreateTimeSheet(string startDate, string endDate, Customer customer, Activity activity, Order order, Faker faker)
        => faker.TimeSheet.Create(customer.Id, activity.Id, null, order?.Id, faker.DateTime.Offset(startDate), faker.DateTime.Offset(endDate));
}

public class WorkTimesPerCustomerTestCase : TestCase
{
    public List<IIdEntityModel> MasterData { get; set; } = new();
    public List<TimeSheet> TimeSheets { get; set; } = new();
    public TimeSheetFilterSet Filters { get; set; } = FakeFilters.Empty();
    public List<object> Expected { get; set; } = new();
}