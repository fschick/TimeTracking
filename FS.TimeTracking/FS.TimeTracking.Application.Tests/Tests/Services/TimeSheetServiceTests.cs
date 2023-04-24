using FakeItEasy;
using FluentAssertions;
using FluentAssertions.Execution;
using FS.TimeTracking.Abstractions.Constants;
using FS.TimeTracking.Abstractions.DTOs.Administration;
using FS.TimeTracking.Abstractions.Enums;
using FS.TimeTracking.Application.Services.Shared;
using FS.TimeTracking.Application.Services.TimeTracking;
using FS.TimeTracking.Application.Tests.Attributes;
using FS.TimeTracking.Application.Tests.Extensions;
using FS.TimeTracking.Application.Tests.Models;
using FS.TimeTracking.Application.Tests.Services;
using FS.TimeTracking.Application.Tests.Services.FakeModels;
using FS.TimeTracking.Core.Exceptions;
using FS.TimeTracking.Core.Interfaces.Application.Services.Shared;
using FS.TimeTracking.Core.Interfaces.Application.Services.TimeTracking;
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
[SuppressMessage("IDisposableAnalyzers.Correctness", "IDISP013:Await in using", Justification = "False positive")]
public class TimeSheetServiceTests
{
    [TestMethod]
    public async Task WhenTimeSheetIsStopped_EndDateIsSet()
    {
        // Prepare
        using var faker = new Faker();
        faker.ConfigureAuthorization(false);

        var timeSheet = faker.TimeSheet.Create(Guid.Empty, Guid.Empty);
        timeSheet.EndDate = null;

        var dbRepository = faker.GetRequiredService<IDbRepository>();
        A.CallTo(() => dbRepository.FirstOrDefault((TimeSheet x) => x, default, default, default, default, default, default))
            .WithAnyArguments()
            .Returns(timeSheet);
        A.CallTo(() => dbRepository.Update(default(TimeSheet)))
            .WithAnyArguments()
            .ReturnsLazily((TimeSheet updatedTimeSheet) => updatedTimeSheet);

        faker.Provide(dbRepository);
        faker.Provide<ITimeSheetApiService, TimeSheetService>();
        var timeSheetService = faker.GetRequiredService<ITimeSheetApiService>();

        // Act
        var stoppedTimeSheet = await timeSheetService.StopTimeSheetEntry(timeSheet.Id, DateTimeOffset.Now);

        // Check
        stoppedTimeSheet.EndDate.Should().Be(timeSheet.EndDate);
    }

    [TestMethod]
    public async Task WhenAlreadyStoppedAndTimeSheetIsStoppedAgain_ExceptionIsThrown()
    {
        // Prepare
        using var faker = new Faker();
        faker.ConfigureAuthorization(false);

        var dbRepository = faker.GetRequiredService<IDbRepository>();
        A.CallTo(() => dbRepository.FirstOrDefault((TimeSheet x) => x, default, default, default, default, default, default))
            .WithAnyArguments()
            .Returns(faker.TimeSheet.Create(Guid.Empty, Guid.Empty));

        faker.Provide(dbRepository);
        faker.Provide<ITimeSheetApiService, TimeSheetService>();
        var timeSheetService = faker.GetRequiredService<ITimeSheetApiService>();

        // Act
        Func<Task> action = async () => await timeSheetService.StopTimeSheetEntry(default, default);

        // Check
        await action.Should().ThrowAsync<InvalidOperationException>().WithMessage("Time sheet with ID * is already stopped.");
    }

    [TestMethod]
    public async Task WhenTimeSheetWithActivityIsAddedAndCustomersDoesNotMatch_ConformityExceptionIsThrown()
    {
        // Prepare
        using var faker = new Faker();
        faker.ConfigureAuthorization(false);
        faker.ConfigureInMemoryDatabase();
        faker.Provide<ITimeSheetApiService, TimeSheetService>();

        var dbRepository = faker.GetRequiredService<IDbRepository>();
        var timeSheetService = faker.GetRequiredService<ITimeSheetApiService>();

        var activityCustomer = faker.Customer.Create();
        var activity = faker.Activity.Create(activityCustomer.Id);
        var timeSheetCustomer = faker.Customer.Create();

        await dbRepository.AddRange(new List<IIdEntityModel> { timeSheetCustomer, activityCustomer, activity });
        await dbRepository.SaveChanges();

        // Act
        var timeSheet = faker.TimeSheet.CreateDto(timeSheetCustomer.Id, activity.Id);
        var createTimeSheet = () => timeSheetService.Create(timeSheet);

        // Check
        await createTimeSheet.Should()
            .ThrowAsync<ConformityException>()
            .WithMessage("Customer of time sheet does not match customer of assigned activity.");
    }

    [TestMethod]
    public async Task WhenTimeSheetWithActivityIsAddedAndActivityHasNoCustomer_NoExceptionIsThrown()
    {
        // Prepare
        using var faker = new Faker();
        faker.ConfigureAuthorization(false);
        faker.ConfigureInMemoryDatabase();
        faker.Provide<ITimeSheetApiService, TimeSheetService>();

        var dbRepository = faker.GetRequiredService<IDbRepository>();
        var timeSheetService = faker.GetRequiredService<ITimeSheetApiService>();

        var activity = faker.Activity.Create();
        var timeSheetCustomer = faker.Customer.Create();

        await dbRepository.AddRange(new List<IIdEntityModel> { timeSheetCustomer, activity });
        await dbRepository.SaveChanges();

        // Act
        var timeSheet = faker.TimeSheet.CreateDto(timeSheetCustomer.Id, activity.Id);
        var createTimeSheet = () => timeSheetService.Create(timeSheet);

        // Check
        await createTimeSheet.Should().NotThrowAsync();
    }

    [TestMethod]
    public async Task WhenTimeSheetWithProjectIsAddedAndProjectHasNoCustomer_NoExceptionIsThrown()
    {
        // Prepare
        using var faker = new Faker();
        faker.ConfigureAuthorization(false);
        faker.ConfigureInMemoryDatabase();
        faker.Provide<ITimeSheetApiService, TimeSheetService>();

        var dbRepository = faker.GetRequiredService<IDbRepository>();
        var timeSheetService = faker.GetRequiredService<ITimeSheetApiService>();

        var project = faker.Project.Create();
        var timeSheetActivity = faker.Activity.Create();
        var timeSheetCustomer = faker.Customer.Create();

        await dbRepository.AddRange(new List<IIdEntityModel> { timeSheetCustomer, timeSheetActivity, project });
        await dbRepository.SaveChanges();

        // Act
        var timeSheet = faker.TimeSheet.CreateDto(timeSheetCustomer.Id, timeSheetActivity.Id, project.Id);
        var createTimeSheet = () => timeSheetService.Create(timeSheet);

        // Check
        await createTimeSheet.Should().NotThrowAsync();
    }

    [TestMethod]
    public async Task WhenTimeSheetWithProjectIsAddedAndCustomersDoesNotMatch_ConformityExceptionIsThrown()
    {
        // Prepare
        using var faker = new Faker();
        faker.ConfigureAuthorization(false);
        faker.ConfigureInMemoryDatabase();
        faker.Provide<ITimeSheetApiService, TimeSheetService>();

        var dbRepository = faker.GetRequiredService<IDbRepository>();
        var timeSheetService = faker.GetRequiredService<ITimeSheetApiService>();

        var projectCustomer = faker.Customer.Create();
        var project = faker.Project.Create(projectCustomer.Id);
        var timeSheetActivity = faker.Activity.Create();
        var timeSheetCustomer = faker.Customer.Create();

        await dbRepository.AddRange(new List<IIdEntityModel> { timeSheetCustomer, timeSheetActivity, projectCustomer, project });
        await dbRepository.SaveChanges();

        // Act
        var timeSheet = faker.TimeSheet.CreateDto(timeSheetCustomer.Id, timeSheetActivity.Id, project.Id);
        var createTimeSheet = () => timeSheetService.Create(timeSheet);

        // Check
        await createTimeSheet.Should()
            .ThrowAsync<ConformityException>()
            .WithMessage("Customer of time sheet does not match customer of assigned project.");
    }

    [TestMethod]
    public async Task WhenTimeSheetWithOrderIsAddedAndCustomersDoesNotMatch_ConformityExceptionIsThrown()
    {
        // Prepare
        using var faker = new Faker();
        faker.ConfigureAuthorization(false);
        faker.ConfigureInMemoryDatabase();
        faker.Provide<ITimeSheetApiService, TimeSheetService>();

        var dbRepository = faker.GetRequiredService<IDbRepository>();
        var timeSheetService = faker.GetRequiredService<ITimeSheetApiService>();

        var orderCustomer = faker.Customer.Create();
        var order = faker.Project.Create(orderCustomer.Id);
        var timeSheetActivity = faker.Activity.Create();
        var timeSheetCustomer = faker.Customer.Create();

        await dbRepository.AddRange(new List<IIdEntityModel> { timeSheetCustomer, timeSheetActivity, orderCustomer, order });
        await dbRepository.SaveChanges();

        // Act
        var timeSheet = faker.TimeSheet.CreateDto(timeSheetCustomer.Id, timeSheetActivity.Id, null, order.Id);
        var createTimeSheet = () => timeSheetService.Create(timeSheet);

        // Check
        await createTimeSheet.Should()
            .ThrowAsync<ConformityException>()
            .WithMessage("Customer of time sheet does not match customer of assigned order.");
    }

    [TestMethod]
    public async Task WhenSimilarTimeSheetEntryIsStarted_UserIdChangedToCurrentUserId()
    {
        // Prepare
        using var faker = new Faker();

        var timeSheetActivity = faker.Activity.Create();
        var timeSheetCustomer = faker.Customer.Create();
        var originUser = faker.User.Create("Origin");
        var currentUser = faker.User.Create("Current", permissions: DefaultPermissions.ReadPermissions);
        var originTimeSheet = faker.TimeSheet.Create(timeSheetCustomer.Id, timeSheetActivity.Id, userId: originUser.Id);

        faker.ConfigureInMemoryDatabase();
        faker.ConfigureAuthorization(true, currentUser);
        faker.Provide<ITimeSheetApiService, TimeSheetService>();

        var dbRepository = faker.GetRequiredService<IDbRepository>();
        var timeSheetService = faker.GetRequiredService<ITimeSheetApiService>();

        await dbRepository.AddRange(new List<IIdEntityModel> { timeSheetCustomer, timeSheetActivity, originTimeSheet });
        await dbRepository.SaveChanges();

        // Act
        var copiedTimeSheet = await timeSheetService.StartSimilarTimeSheetEntry(originTimeSheet.Id, originTimeSheet.StartDate);

        // Check
        copiedTimeSheet.UserId.Should().Be(currentUser.Id);
    }

    [DataTestMethod]
    [WorkedDaysOverviewDataSource]
    public async Task WhenWorkedDaysOverviewRequested_ResultMatchesExpectedValues(string testCaseJson)
    {
        // Prepare
        var testCase = TestCase.FromJson<WorkedDaysOverviewTestCase>(testCaseJson);

        using var faker = new Faker();
        faker.ConfigureInMemoryDatabase();
        faker.ConfigureAuthorization(true, testCase.CurrentUser);
        faker.Provide<IWorkdayService, WorkdayService>();
        faker.Provide<ITimeSheetApiService, TimeSheetService>();

        var dbRepository = faker.GetRequiredService<IDbRepository>();
        await dbRepository.AddRange(testCase.MasterData);
        await dbRepository.AddRange(testCase.Holidays);
        await dbRepository.AddRange(testCase.TimeSheets);
        await dbRepository.SaveChanges();

        await faker.ProvideUsers(testCase.Users.ToArray());

        // Act
        var timeSheetService = faker.GetRequiredService<ITimeSheetApiService>();
        var workDayOverview = await timeSheetService.GetWorkedDaysOverview(testCase.Filters);

        // Check
        using var _ = new AssertionScope();
        workDayOverview.Should().BeEquivalentTo(testCase.Expected);
    }

    public class WorkedDaysOverviewDataSourceAttribute : TestCaseDataSourceAttribute
    {
        public WorkedDaysOverviewDataSourceAttribute() : base(GetTestCases()) { }

        private static List<TestCase> GetTestCases()
        {
            using var faker = new Faker();
            var currentUser = faker.User.Create("Current", permissions: DefaultPermissions.ReadPermissions);
            var userEve = faker.User.Create("Eve");
            var userBob = faker.User.Create("Bob");
            var allUsers = new List<UserDto> { currentUser, userEve, userBob };
            var customer = faker.Customer.Create();
            var activity = faker.Activity.Create();
            var masterData = new List<IIdEntityModel> { customer, activity };

            return new List<TestCase>
            {
                new WorkedDaysOverviewTestCase
                {
                    Identifier = "NoPublicHolidaysNoVacation",
                    CurrentUser = currentUser,
                    Users = allUsers,
                    MasterData = masterData,
                    TimeSheets = new List<TimeSheet> { CreateTimeSheet("2020-06-01 03:00", "2020-06-01 04:00", customer, activity, currentUser, faker) },
                    Expected = new { TotalTimeWorked = TimeSpan.FromHours(1), PersonalWorkdays = 3, PublicWorkdays = 3 },
                },
                new WorkedDaysOverviewTestCase
                {
                    Identifier = "NoPublicHolidaysButVacation",
                    CurrentUser = currentUser,
                    Users = allUsers,
                    MasterData = masterData,
                    TimeSheets = new List<TimeSheet> {
                        CreateTimeSheet("2020-06-01 03:00", "2020-06-01 04:00", customer, activity, currentUser, faker),
                        CreateTimeSheet("2020-06-03 03:00", "2020-06-03 04:00", customer, activity, currentUser, faker),
                    },
                    Holidays = new List<Holiday> { faker.Holiday.Create("2020-06-02", "2020-06-02", HolidayType.Holiday, currentUser.Id)  },
                    Expected = new { TotalTimeWorked = TimeSpan.FromHours(2), PersonalWorkdays = 8, PublicWorkdays = 9 },
                },
                new WorkedDaysOverviewTestCase
                {
                    Identifier = "PublicHolidaysButNoVacation",
                    CurrentUser = currentUser,
                    Users = allUsers,
                    MasterData = masterData,
                    TimeSheets = new List<TimeSheet> {
                        CreateTimeSheet("2020-06-01 03:00", "2020-06-01 04:00", customer, activity, currentUser, faker),
                        CreateTimeSheet("2020-06-03 03:00", "2020-06-03 04:00", customer, activity, currentUser, faker),
                    },
                    Holidays = new List<Holiday> { faker.Holiday.Create("2020-06-02", "2020-06-02", HolidayType.PublicHoliday)  },
                    Expected = new { TotalTimeWorked = TimeSpan.FromHours(2), PersonalWorkdays = 6, PublicWorkdays = 6 },
                },
                new WorkedDaysOverviewTestCase
                {
                    Identifier = "PublicHolidaysSameDayVacation",
                    CurrentUser = currentUser,
                    Users = allUsers,
                    MasterData = masterData,
                    TimeSheets = new List<TimeSheet> {
                        CreateTimeSheet("2020-06-01 03:00", "2020-06-01 04:00", customer, activity, currentUser, faker),
                        CreateTimeSheet("2020-06-03 03:00", "2020-06-03 04:00", customer, activity, currentUser, faker),
                    },
                    Holidays = new List<Holiday> {
                        faker.Holiday.Create("2020-06-02", "2020-06-02", HolidayType.PublicHoliday),
                        faker.Holiday.Create("2020-06-02", "2020-06-02", HolidayType.Holiday, currentUser.Id),
                    },
                    Expected = new { TotalTimeWorked = TimeSpan.FromHours(2), PersonalWorkdays = 6, PublicWorkdays = 6 },
                },
                new WorkedDaysOverviewTestCase
                {
                    Identifier = "PublicHolidaysOverlappingVacation",
                    CurrentUser = currentUser,
                    Users = allUsers,
                    MasterData = masterData,
                    TimeSheets = new List<TimeSheet> {
                        CreateTimeSheet("2020-06-01 03:00", "2020-06-01 04:00", customer, activity, currentUser, faker),
                        CreateTimeSheet("2020-06-03 03:00", "2020-06-03 04:00", customer, activity, currentUser, faker),
                    },
                    Holidays = new List<Holiday> {
                        faker.Holiday.Create("2020-06-02", "2020-06-02", HolidayType.PublicHoliday),
                        faker.Holiday.Create("2020-06-01", "2020-06-03", HolidayType.Holiday, currentUser.Id),
                    },
                    Expected = new { TotalTimeWorked = TimeSpan.FromHours(2), PersonalWorkdays = 4, PublicWorkdays = 6 },
                },
                new WorkedDaysOverviewTestCase
                {
                    Identifier = "NoTimeSheetsButFiltered",
                    CurrentUser = currentUser,
                    Users = allUsers,
                    Filters = faker.Filters.Create("<2020-06-04", ">=2020-06-02"),
                    Expected = new { TotalTimeWorked = TimeSpan.FromHours(0), PersonalWorkdays = 6, PublicWorkdays = 6 },
                },
                new WorkedDaysOverviewTestCase
                {
                    Identifier = "CutFirstByFilter",
                    CurrentUser = currentUser,
                    Users = allUsers,
                    MasterData = masterData,
                    TimeSheets = new List<TimeSheet> {
                        CreateTimeSheet("2020-06-01 03:00", "2020-06-01 04:00", customer, activity, currentUser, faker),
                        CreateTimeSheet("2020-06-03 03:00", "2020-06-03 04:00", customer, activity, currentUser, faker),
                    },
                    Filters = faker.Filters.Create("<2020-06-04", ">=2020-06-02"),
                    Expected = new { TotalTimeWorked = TimeSpan.FromHours(1), PersonalWorkdays = 6, PublicWorkdays = 6 },
                },
                new WorkedDaysOverviewTestCase
                {
                    Identifier = "CutLastByFilter",
                    CurrentUser = currentUser,
                    Users = allUsers,
                    MasterData = masterData,
                    TimeSheets = new List<TimeSheet> {
                        CreateTimeSheet("2020-06-01 03:00", "2020-06-01 04:00", customer, activity, currentUser, faker),
                        CreateTimeSheet("2020-06-03 03:00", "2020-06-03 04:00", customer, activity, currentUser, faker),
                    },
                    Filters = faker.Filters.Create("<2020-06-03", ">=2020-06-01"),
                    Expected = new { TotalTimeWorked = TimeSpan.FromHours(1), PersonalWorkdays = 6, PublicWorkdays = 6 },
                },
                new WorkedDaysOverviewTestCase
                {
                    Identifier = "BeforeStartOfDaylightSavingTime",
                    CurrentUser = currentUser,
                    Users = allUsers,
                    MasterData = masterData,
                    TimeSheets = new List<TimeSheet> { CreateTimeSheet("2020-03-28 00:30", "2020-03-28 03:30", customer, activity, currentUser, faker) },
                    Expected = new { TotalTimeWorked = TimeSpan.FromHours(3), PersonalWorkdays = 0, PublicWorkdays = 0 },
                },
                new WorkedDaysOverviewTestCase
                {
                    Identifier = "WhileStartOfDaylightSavingTime",
                    CurrentUser = currentUser,
                    Users = allUsers,
                    MasterData = masterData,
                    TimeSheets = new List<TimeSheet> { CreateTimeSheet("2020-03-29 00:30", "2020-03-29 03:30", customer, activity, currentUser, faker) },
                    Expected = new { TotalTimeWorked = TimeSpan.FromHours(2), PersonalWorkdays = 0, PublicWorkdays = 0 },
                },
                new WorkedDaysOverviewTestCase
                {
                    Identifier = "WhileEndOfDaylightSavingTime",
                    CurrentUser = currentUser,
                    Users = allUsers,
                    MasterData = masterData,
                    TimeSheets = new List<TimeSheet> { CreateTimeSheet("2020-10-25 00:30", "2020-10-25 03:30", customer, activity, currentUser, faker) },
                    Expected = new { TotalTimeWorked = TimeSpan.FromHours(4), PersonalWorkdays = 0, PublicWorkdays = 0 },
                },
                new WorkedDaysOverviewTestCase
                {
                    Identifier = "UserFiltered",
                    CurrentUser = currentUser,
                    Users = allUsers,
                    MasterData = masterData,
                    TimeSheets = new List<TimeSheet> {
                        CreateTimeSheet("2020-06-01 03:00", "2020-06-01 04:00", customer, activity, userEve, faker),
                        CreateTimeSheet("2020-06-03 03:00", "2020-06-03 04:00", customer, activity, userBob, faker),
                    },
                    Filters = faker.Filters.Create(userEve),
                    Expected = new { TotalTimeWorked = TimeSpan.FromHours(1), PersonalWorkdays = 1, PublicWorkdays = 1 },
                },
            };
        }

        public static TimeSheet CreateTimeSheet(string startDate, string endDate, Customer customer, Activity activity, UserDto user, Faker faker)
            => faker.TimeSheet.Create(customer.Id, activity.Id, null, null, faker.DateTime.Offset(startDate), faker.DateTime.Offset(endDate), null, user.Id);
    }

    public class WorkedDaysOverviewTestCase : TestCase
    {
        public UserDto CurrentUser { get; set; } = new();
        public List<UserDto> Users { get; set; } = new();
        public List<IIdEntityModel> MasterData { get; set; } = new();
        public List<TimeSheet> TimeSheets { get; set; } = new();
        public List<Holiday> Holidays { get; set; } = new();
        public TimeSheetFilterSet Filters { get; set; } = FakeFilters.Empty();
        public object Expected { get; set; } = new();
    }
}