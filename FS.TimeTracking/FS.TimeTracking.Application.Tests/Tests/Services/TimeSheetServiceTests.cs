using Autofac.Extras.FakeItEasy;
using FakeItEasy;
using FluentAssertions;
using FluentAssertions.Execution;
using FS.TimeTracking.Abstractions.DTOs.Administration;
using FS.TimeTracking.Abstractions.Enums;
using FS.TimeTracking.Application.Services.Shared;
using FS.TimeTracking.Application.Services.TimeTracking;
using FS.TimeTracking.Application.Tests.Attributes;
using FS.TimeTracking.Application.Tests.Extensions;
using FS.TimeTracking.Application.Tests.Models;
using FS.TimeTracking.Application.Tests.Services;
using FS.TimeTracking.Core.Exceptions;
using FS.TimeTracking.Core.Interfaces.Application.Services.Administration;
using FS.TimeTracking.Core.Interfaces.Application.Services.Shared;
using FS.TimeTracking.Core.Interfaces.Application.Services.TimeTracking;
using FS.TimeTracking.Core.Interfaces.Models;
using FS.TimeTracking.Core.Interfaces.Repository.Services.Database;
using FS.TimeTracking.Core.Models.Application.MasterData;
using FS.TimeTracking.Core.Models.Application.TimeTracking;
using FS.TimeTracking.Core.Models.Configuration;
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
public class TimeSheetServiceTests
{
    [TestMethod]
    public async Task WhenTimeSheetIsStopped_EndDateIsSet()
    {
        // Prepare
        var faker = new Faker(2000);
        using var autoFake = new AutoFake();

        var timeSheet = faker.TimeSheet.Create(Guid.Empty, Guid.Empty);
        timeSheet.EndDate = null;

        var dbRepository = autoFake.Resolve<IDbRepository>();
        A.CallTo(() => dbRepository.FirstOrDefault((TimeSheet x) => x, default, default, default, default, default, default))
            .WithAnyArguments()
            .Returns(timeSheet);
        A.CallTo(() => dbRepository.Update(default(TimeSheet)))
            .WithAnyArguments()
            .ReturnsLazily((TimeSheet updatedTimeSheet) => updatedTimeSheet);

        autoFake.Provide(faker.AutoMapper);
        autoFake.Provide(dbRepository);
        autoFake.Provide<ITimeSheetService, TimeSheetService>();
        var timeSheetService = autoFake.Resolve<ITimeSheetService>();

        // Act
        var stoppedTimeSheet = await timeSheetService.StopTimeSheetEntry(timeSheet.Id, DateTimeOffset.Now);

        // Check
        stoppedTimeSheet.EndDate.Should().Be(timeSheet.EndDate);
    }

    [TestMethod]
    public async Task WhenAlreadyStoppedAndTimeSheetIsStoppedAgain_ExceptionIsThrown()
    {
        // Prepare
        var faker = new Faker(2000);
        using var autoFake = new AutoFake();

        var dbRepository = autoFake.Resolve<IDbRepository>();
        A.CallTo(() => dbRepository.FirstOrDefault((TimeSheet x) => x, default, default, default, default, default, default))
            .WithAnyArguments()
            .Returns(faker.TimeSheet.Create(Guid.Empty, Guid.Empty));

        autoFake.Provide(dbRepository);
        autoFake.Provide<ITimeSheetService, TimeSheetService>();
        var timeSheetService = autoFake.Resolve<ITimeSheetService>();

        // Act
        Func<Task> action = async () => await timeSheetService.StopTimeSheetEntry(default, default);

        // Check
        await action.Should().ThrowAsync<InvalidOperationException>().WithMessage("Time sheet with ID * is already stopped.");
    }

    [TestMethod]
    public async Task WhenTimeSheetWithActivityIsAddedAndCustomersDoesNotMatch_ConformityExceptionIsThrown()
    {
        // Prepare
        var faker = new Faker(2000);

        using var autoFake = new AutoFake();
        await autoFake.ConfigureInMemoryDatabase();
        autoFake.Provide(faker.AutoMapper);
        autoFake.Provide<IDbRepository, DbRepository<TimeTrackingDbContext>>();
        autoFake.Provide<ITimeSheetService, TimeSheetService>();

        var dbRepository = autoFake.Resolve<IDbRepository>();
        var timeSheetService = autoFake.Resolve<ITimeSheetService>();

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
        var faker = new Faker(2000);
        using var autoFake = new AutoFake();
        await autoFake.ConfigureInMemoryDatabase();
        autoFake.Provide(faker.AutoMapper);
        autoFake.Provide<IDbRepository, DbRepository<TimeTrackingDbContext>>();
        autoFake.Provide<ITimeSheetService, TimeSheetService>();

        var dbRepository = autoFake.Resolve<IDbRepository>();
        var timeSheetService = autoFake.Resolve<ITimeSheetService>();

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
        var faker = new Faker(2000);
        using var autoFake = new AutoFake();
        await autoFake.ConfigureInMemoryDatabase();
        autoFake.Provide(faker.AutoMapper);
        autoFake.Provide<IDbRepository, DbRepository<TimeTrackingDbContext>>();
        autoFake.Provide<ITimeSheetService, TimeSheetService>();

        var dbRepository = autoFake.Resolve<IDbRepository>();
        var timeSheetService = autoFake.Resolve<ITimeSheetService>();

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
        var faker = new Faker(2000);
        using var autoFake = new AutoFake();
        await autoFake.ConfigureInMemoryDatabase();
        autoFake.Provide(faker.AutoMapper);
        autoFake.Provide<IDbRepository, DbRepository<TimeTrackingDbContext>>();
        autoFake.Provide<ITimeSheetService, TimeSheetService>();

        var dbRepository = autoFake.Resolve<IDbRepository>();
        var timeSheetService = autoFake.Resolve<ITimeSheetService>();

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
        var faker = new Faker(2000);
        using var autoFake = new AutoFake();
        await autoFake.ConfigureInMemoryDatabase();
        autoFake.Provide(faker.AutoMapper);
        autoFake.Provide<IDbRepository, DbRepository<TimeTrackingDbContext>>();
        autoFake.Provide<ITimeSheetService, TimeSheetService>();

        var dbRepository = autoFake.Resolve<IDbRepository>();
        var timeSheetService = autoFake.Resolve<ITimeSheetService>();

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

    [DataTestMethod]
    [WorkedDaysOverviewDataSource]
    public async Task WhenWorkedDaysOverviewRequested_ResultMatchesExpectedValues(string testCaseJson)
    {
        // Prepare
        var testCase = TestCase.FromJson<WorkedDaysOverviewTestCase>(testCaseJson);

        var faker = new Faker(2000);
        using var autoFake = new AutoFake();
        await autoFake.ConfigureInMemoryDatabase(new TimeTrackingConfiguration { Features = { Authorization = true } });
        autoFake.Provide(faker.AutoMapper);
        autoFake.Provide<IFilterFactory, FilterFactory>();
        autoFake.Provide<IDbRepository, DbRepository<TimeTrackingDbContext>>();
        autoFake.Provide<IUserService, UserServiceInMemory>();
        autoFake.Provide<IWorkdayService, WorkdayService>();
        autoFake.Provide<ITimeSheetService, TimeSheetService>();

        var dbRepository = autoFake.Resolve<IDbRepository>();
        await dbRepository.AddRange(testCase.MasterData);
        await dbRepository.AddRange(testCase.Holidays);
        await dbRepository.AddRange(testCase.TimeSheets);
        await dbRepository.SaveChanges();

        var userService = autoFake.Resolve<IUserService>();
        foreach (var user in testCase.Users)
            await userService.Create(user);

        // Act
        var timeSheetService = autoFake.Resolve<ITimeSheetService>();
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
            var faker = new Faker(2000);
            var customer = faker.Customer.Create();
            var activity = faker.Activity.Create();
            var masterData = new List<IIdEntityModel> { customer, activity };
            var user1 = faker.User.Create("Eve");
            var user2 = faker.User.Create("Bob");
            var users = new List<UserDto> { user1, user2 };

            return new List<TestCase>
            {
                new WorkedDaysOverviewTestCase
                {
                    Identifier = "NoPublicHolidaysNoVacation",
                    MasterData = masterData,
                    TimeSheets = new List<TimeSheet> { CreateTimeSheet("2020-06-01 03:00", "2020-06-01 04:00", faker, customer, activity, user1) },
                    Expected = new { TotalTimeWorked = TimeSpan.FromHours(1), PersonalWorkdays = 1, PublicWorkdays = 1 },
                },
                new WorkedDaysOverviewTestCase
                {
                    Identifier = "NoPublicHolidaysButVacation",
                    MasterData = masterData,
                    TimeSheets = new List<TimeSheet> {
                        CreateTimeSheet("2020-06-01 03:00", "2020-06-01 04:00", faker, customer, activity, user1),
                        CreateTimeSheet("2020-06-03 03:00", "2020-06-03 04:00", faker, customer, activity, user1),
                    },
                    Holidays = new List<Holiday> { faker.Holiday.Create("2020-06-02", "2020-06-02", HolidayType.Holiday)  },
                    Expected = new { TotalTimeWorked = TimeSpan.FromHours(2), PersonalWorkdays = 2, PublicWorkdays = 3 },
                },
                new WorkedDaysOverviewTestCase
                {
                    Identifier = "PublicHolidaysButNoVacation",
                    MasterData = masterData,
                    TimeSheets = new List<TimeSheet> {
                        CreateTimeSheet("2020-06-01 03:00", "2020-06-01 04:00", faker, customer, activity, user1),
                        CreateTimeSheet("2020-06-03 03:00", "2020-06-03 04:00", faker, customer, activity, user1),
                    },
                    Holidays = new List<Holiday> { faker.Holiday.Create("2020-06-02", "2020-06-02", HolidayType.PublicHoliday)  },
                    Expected = new { TotalTimeWorked = TimeSpan.FromHours(2), PersonalWorkdays = 2, PublicWorkdays = 2 },
                },
                new WorkedDaysOverviewTestCase
                {
                    Identifier = "PublicHolidaysSameDayVacation",
                    MasterData = masterData,
                    TimeSheets = new List<TimeSheet> {
                        CreateTimeSheet("2020-06-01 03:00", "2020-06-01 04:00", faker, customer, activity, user1),
                        CreateTimeSheet("2020-06-03 03:00", "2020-06-03 04:00", faker, customer, activity, user1),
                    },
                    Holidays = new List<Holiday> {
                        faker.Holiday.Create("2020-06-02", "2020-06-02", HolidayType.PublicHoliday),
                        faker.Holiday.Create("2020-06-02", "2020-06-02", HolidayType.Holiday),
                    },
                    Expected = new { TotalTimeWorked = TimeSpan.FromHours(2), PersonalWorkdays = 2, PublicWorkdays = 2 },
                },
                new WorkedDaysOverviewTestCase
                {
                    Identifier = "PublicHolidaysOverlappingVacation",
                    MasterData = masterData,
                    TimeSheets = new List<TimeSheet> {
                        CreateTimeSheet("2020-06-01 03:00", "2020-06-01 04:00", faker, customer, activity, user1),
                        CreateTimeSheet("2020-06-03 03:00", "2020-06-03 04:00", faker, customer, activity, user1),
                    },
                    Holidays = new List<Holiday> {
                        faker.Holiday.Create("2020-06-02", "2020-06-02", HolidayType.PublicHoliday),
                        faker.Holiday.Create("2020-06-01", "2020-06-03", HolidayType.Holiday),
                    },
                    Expected = new { TotalTimeWorked = TimeSpan.FromHours(2), PersonalWorkdays = 0, PublicWorkdays = 2 },
                },
                new WorkedDaysOverviewTestCase
                {
                    Identifier = "NoTimeSheetsButFiltered",
                    Filters = faker.Filters.Create("<2020-06-04", ">=2020-06-02"),
                    Expected = new { TotalTimeWorked = TimeSpan.FromHours(0), PersonalWorkdays = 2, PublicWorkdays = 2 },
                },
                new WorkedDaysOverviewTestCase
                {
                    Identifier = "CutFirstByFilter",
                    MasterData = masterData,
                    TimeSheets = new List<TimeSheet> {
                        CreateTimeSheet("2020-06-01 03:00", "2020-06-01 04:00", faker, customer, activity, user1),
                        CreateTimeSheet("2020-06-03 03:00", "2020-06-03 04:00", faker, customer, activity, user1),
                    },
                    Filters = faker.Filters.Create("<2020-06-04", ">=2020-06-02"),
                    Expected = new { TotalTimeWorked = TimeSpan.FromHours(1), PersonalWorkdays = 2, PublicWorkdays = 2 },
                },
                new WorkedDaysOverviewTestCase
                {
                    Identifier = "CutLastByFilter",
                    MasterData = masterData,
                    TimeSheets = new List<TimeSheet> {
                        CreateTimeSheet("2020-06-01 03:00", "2020-06-01 04:00", faker, customer, activity, user1),
                        CreateTimeSheet("2020-06-03 03:00", "2020-06-03 04:00", faker, customer, activity, user1),
                    },
                    Filters = faker.Filters.Create("<2020-06-03", ">=2020-06-01"),
                    Expected = new { TotalTimeWorked = TimeSpan.FromHours(1), PersonalWorkdays = 2, PublicWorkdays = 2 },
                },
                new WorkedDaysOverviewTestCase
                {
                    Identifier = "BeforeStartOfDaylightSavingTime",
                    MasterData = masterData,
                    TimeSheets = new List<TimeSheet> { CreateTimeSheet("2020-03-28 00:30", "2020-03-28 03:30", faker, customer, activity, user1) },
                    Expected = new { TotalTimeWorked = TimeSpan.FromHours(3), PersonalWorkdays = 0, PublicWorkdays = 0 },
                },
                new WorkedDaysOverviewTestCase
                {
                    Identifier = "WhileStartOfDaylightSavingTime",
                    MasterData = masterData,
                    TimeSheets = new List<TimeSheet> { CreateTimeSheet("2020-03-29 00:30", "2020-03-29 03:30", faker, customer, activity, user1) },
                    Expected = new { TotalTimeWorked = TimeSpan.FromHours(2), PersonalWorkdays = 0, PublicWorkdays = 0 },
                },
                new WorkedDaysOverviewTestCase
                {
                    Identifier = "WhileEndOfDaylightSavingTime",
                    MasterData = masterData,
                    TimeSheets = new List<TimeSheet> { CreateTimeSheet("2020-10-25 00:30", "2020-10-25 03:30", faker, customer, activity, user1) },
                    Expected = new { TotalTimeWorked = TimeSpan.FromHours(4), PersonalWorkdays = 0, PublicWorkdays = 0 },
                },
                new WorkedDaysOverviewTestCase
                {
                    Identifier = "UserFiltered",
                    MasterData = masterData,
                    Users = users,
                    TimeSheets = new List<TimeSheet> {
                        CreateTimeSheet("2020-06-01 03:00", "2020-06-01 04:00", faker, customer, activity, user1),
                        CreateTimeSheet("2020-06-03 03:00", "2020-06-03 04:00", faker, customer, activity, user2),
                    },
                    Filters = faker.Filters.Create(user1),
                    Expected = new { TotalTimeWorked = TimeSpan.FromHours(1), PersonalWorkdays = 1, PublicWorkdays = 1 },
                },
            };
        }

        public static TimeSheet CreateTimeSheet(string startDate, string endDate, Faker faker, Customer customer, Activity activity, UserDto user)
            => faker.TimeSheet.Create(customer.Id, activity.Id, null, null, faker.DateTime.Offset(startDate), faker.DateTime.Offset(endDate), null, user.Id);
    }

    public class WorkedDaysOverviewTestCase : TestCase
    {
        public List<IIdEntityModel> MasterData { get; set; } = new();
        public List<UserDto> Users { get; set; } = new();
        public List<TimeSheet> TimeSheets { get; set; } = new();
        public List<Holiday> Holidays { get; set; } = new();
        public TimeSheetFilterSet Filters { get; set; } = FakeFilters.Empty();
        public object Expected { get; set; } = new();
    }
}