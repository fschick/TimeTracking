using Autofac.Extras.FakeItEasy;
using FakeItEasy;
using FluentAssertions;
using FluentAssertions.Execution;
using FS.TimeTracking.Abstractions.Enums;
using FS.TimeTracking.Application.Services.Shared;
using FS.TimeTracking.Application.Services.TimeTracking;
using FS.TimeTracking.Application.Tests.Attributes;
using FS.TimeTracking.Application.Tests.Extensions;
using FS.TimeTracking.Application.Tests.Models;
using FS.TimeTracking.Application.Tests.Services;
using FS.TimeTracking.Core.Exceptions;
using FS.TimeTracking.Core.Interfaces.Application.Services.Shared;
using FS.TimeTracking.Core.Interfaces.Application.Services.TimeTracking;
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
public class TimeSheetServiceTests
{
    [TestMethod]
    public async Task WhenTimeSheetIsStopped_EndDateIsSet()
    {
        // Prepare
        using var autoFake = new AutoFake();

        var timeSheet = FakeTimeSheet.Create(Guid.Empty, Guid.Empty);
        timeSheet.EndDate = null;

        var repository = autoFake.Resolve<IRepository>();
        A.CallTo(() => repository.FirstOrDefault((TimeSheet x) => x, default, default, default, default, default, default))
            .WithAnyArguments()
            .Returns(timeSheet);
        A.CallTo(() => repository.Update(default(TimeSheet)))
            .WithAnyArguments()
            .ReturnsLazily((TimeSheet updatedTimeSheet) => updatedTimeSheet);

        autoFake.Provide(FakeAutoMapper.Mapper);
        autoFake.Provide(repository);
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
        using var autoFake = new AutoFake();

        var repository = autoFake.Resolve<IRepository>();
        A.CallTo(() => repository.FirstOrDefault((TimeSheet x) => x, default, default, default, default, default, default))
            .WithAnyArguments()
            .Returns(FakeTimeSheet.Create(Guid.Empty, Guid.Empty));

        autoFake.Provide(repository);
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
        using var autoFake = new AutoFake();
        await autoFake.ConfigureInMemoryDatabase();
        autoFake.Provide(FakeAutoMapper.Mapper);
        autoFake.Provide<IRepository, Repository<TimeTrackingDbContext>>();
        autoFake.Provide<ITimeSheetService, TimeSheetService>();

        var repository = autoFake.Resolve<IRepository>();
        var timeSheetService = autoFake.Resolve<ITimeSheetService>();

        var activityCustomer = FakeCustomer.Create();
        var activity = FakeActivity.Create(activityCustomer.Id);
        var timeSheetCustomer = FakeCustomer.Create();

        await repository.AddRange(new List<IIdEntityModel> { timeSheetCustomer, activityCustomer, activity });
        await repository.SaveChanges();

        // Act
        var timeSheet = FakeTimeSheet.CreateDto(timeSheetCustomer.Id, activity.Id);
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
        using var autoFake = new AutoFake();
        await autoFake.ConfigureInMemoryDatabase();
        autoFake.Provide(FakeAutoMapper.Mapper);
        autoFake.Provide<IRepository, Repository<TimeTrackingDbContext>>();
        autoFake.Provide<ITimeSheetService, TimeSheetService>();

        var repository = autoFake.Resolve<IRepository>();
        var timeSheetService = autoFake.Resolve<ITimeSheetService>();

        var activity = FakeActivity.Create();
        var timeSheetCustomer = FakeCustomer.Create();

        await repository.AddRange(new List<IIdEntityModel> { timeSheetCustomer, activity });
        await repository.SaveChanges();

        // Act
        var timeSheet = FakeTimeSheet.CreateDto(timeSheetCustomer.Id, activity.Id);
        var createTimeSheet = () => timeSheetService.Create(timeSheet);

        // Check
        await createTimeSheet.Should().NotThrowAsync<ConformityException>();
    }

    [TestMethod]
    public async Task WhenTimeSheetWithProjectIsAddedAndProjectHasNoCustomer_NoExceptionIsThrown()
    {
        // Prepare
        using var autoFake = new AutoFake();
        await autoFake.ConfigureInMemoryDatabase();
        autoFake.Provide(FakeAutoMapper.Mapper);
        autoFake.Provide<IRepository, Repository<TimeTrackingDbContext>>();
        autoFake.Provide<ITimeSheetService, TimeSheetService>();

        var repository = autoFake.Resolve<IRepository>();
        var timeSheetService = autoFake.Resolve<ITimeSheetService>();

        var project = FakeProject.Create();
        var timeSheetActivity = FakeActivity.Create();
        var timeSheetCustomer = FakeCustomer.Create();

        await repository.AddRange(new List<IIdEntityModel> { timeSheetCustomer, timeSheetActivity, project });
        await repository.SaveChanges();

        // Act
        var timeSheet = FakeTimeSheet.CreateDto(timeSheetCustomer.Id, timeSheetActivity.Id, project.Id);
        var createTimeSheet = () => timeSheetService.Create(timeSheet);

        // Check
        await createTimeSheet.Should().NotThrowAsync<ConformityException>();
    }

    [TestMethod]
    public async Task WhenTimeSheetWithProjectIsAddedAndCustomersDoesNotMatch_ConformityExceptionIsThrown()
    {
        // Prepare
        using var autoFake = new AutoFake();
        await autoFake.ConfigureInMemoryDatabase();
        autoFake.Provide(FakeAutoMapper.Mapper);
        autoFake.Provide<IRepository, Repository<TimeTrackingDbContext>>();
        autoFake.Provide<ITimeSheetService, TimeSheetService>();

        var repository = autoFake.Resolve<IRepository>();
        var timeSheetService = autoFake.Resolve<ITimeSheetService>();

        var projectCustomer = FakeCustomer.Create();
        var project = FakeProject.Create(projectCustomer.Id);
        var timeSheetActivity = FakeActivity.Create();
        var timeSheetCustomer = FakeCustomer.Create();

        await repository.AddRange(new List<IIdEntityModel> { timeSheetCustomer, timeSheetActivity, projectCustomer, project });
        await repository.SaveChanges();

        // Act
        var timeSheet = FakeTimeSheet.CreateDto(timeSheetCustomer.Id, timeSheetActivity.Id, project.Id);
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
        using var autoFake = new AutoFake();
        await autoFake.ConfigureInMemoryDatabase();
        autoFake.Provide(FakeAutoMapper.Mapper);
        autoFake.Provide<IRepository, Repository<TimeTrackingDbContext>>();
        autoFake.Provide<ITimeSheetService, TimeSheetService>();

        var repository = autoFake.Resolve<IRepository>();
        var timeSheetService = autoFake.Resolve<ITimeSheetService>();

        var orderCustomer = FakeCustomer.Create();
        var order = FakeProject.Create(orderCustomer.Id);
        var timeSheetActivity = FakeActivity.Create();
        var timeSheetCustomer = FakeCustomer.Create();

        await repository.AddRange(new List<IIdEntityModel> { timeSheetCustomer, timeSheetActivity, orderCustomer, order });
        await repository.SaveChanges();

        // Act
        var timeSheet = FakeTimeSheet.CreateDto(timeSheetCustomer.Id, timeSheetActivity.Id, null, order.Id);
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

        using var autoFake = new AutoFake();
        await autoFake.ConfigureInMemoryDatabase();
        autoFake.Provide(FakeAutoMapper.Mapper);
        autoFake.Provide<IRepository, Repository<TimeTrackingDbContext>>();
        autoFake.Provide<IWorkdayService, WorkdayService>();
        autoFake.Provide<ITimeSheetService, TimeSheetService>();

        var repository = autoFake.Resolve<IRepository>();
        await repository.AddRange(testCase.MasterData);
        await repository.AddRange(testCase.Holidays);
        await repository.AddRange(testCase.TimeSheets);
        await repository.SaveChanges();

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
            var customer = FakeCustomer.Create();
            var activity = FakeActivity.Create();
            var masterData = new List<IIdEntityModel> { customer, activity };

            return new List<TestCase>
            {
                new WorkedDaysOverviewTestCase
                {
                    Identifier = "No_PublicHolidays_No_Vacation",
                    MasterData = masterData,
                    TimeSheets = new List<TimeSheet> { CreateTimeSheet(customer, activity, "2020-06-01 03:00", "2020-06-01 04:00") },
                    Expected = new { TotalTimeWorked = TimeSpan.FromHours(1), PersonalWorkdays = 1, PublicWorkdays = 1 },
                },
                new WorkedDaysOverviewTestCase
                {
                    Identifier = "No_PublicHolidays_But_Vacation",
                    MasterData = masterData,
                    TimeSheets = new List<TimeSheet> {
                        CreateTimeSheet(customer, activity, "2020-06-01 03:00", "2020-06-01 04:00"),
                        CreateTimeSheet(customer, activity, "2020-06-03 03:00", "2020-06-03 04:00"),
                    },
                    Holidays = new List<Holiday> { FakeHoliday.Create("2020-06-02", "2020-06-02", HolidayType.Holiday)  },
                    Expected = new { TotalTimeWorked = TimeSpan.FromHours(2), PersonalWorkdays = 2, PublicWorkdays = 3 },
                },
                new WorkedDaysOverviewTestCase
                {
                    Identifier = "PublicHolidays_But_No_Vacation",
                    MasterData = masterData,
                    TimeSheets = new List<TimeSheet> {
                        CreateTimeSheet(customer, activity, "2020-06-01 03:00", "2020-06-01 04:00"),
                        CreateTimeSheet(customer, activity, "2020-06-03 03:00", "2020-06-03 04:00"),
                    },
                    Holidays = new List<Holiday> { FakeHoliday.Create("2020-06-02", "2020-06-02", HolidayType.PublicHoliday)  },
                    Expected = new { TotalTimeWorked = TimeSpan.FromHours(2), PersonalWorkdays = 2, PublicWorkdays = 2 },
                },
                new WorkedDaysOverviewTestCase
                {
                    Identifier = "PublicHolidays_SameDay_Vacation",
                    MasterData = masterData,
                    TimeSheets = new List<TimeSheet> {
                        CreateTimeSheet(customer, activity, "2020-06-01 03:00", "2020-06-01 04:00"),
                        CreateTimeSheet(customer, activity, "2020-06-03 03:00", "2020-06-03 04:00"),
                    },
                    Holidays = new List<Holiday> {
                        FakeHoliday.Create("2020-06-02", "2020-06-02", HolidayType.PublicHoliday),
                        FakeHoliday.Create("2020-06-02", "2020-06-02", HolidayType.Holiday),
                    },
                    Expected = new { TotalTimeWorked = TimeSpan.FromHours(2), PersonalWorkdays = 2, PublicWorkdays = 2 },
                },
                new WorkedDaysOverviewTestCase
                {
                    Identifier = "PublicHolidays_Overlapping_Vacation",
                    MasterData = masterData,
                    TimeSheets = new List<TimeSheet> {
                        CreateTimeSheet(customer, activity, "2020-06-01 03:00", "2020-06-01 04:00"),
                        CreateTimeSheet(customer, activity, "2020-06-03 03:00", "2020-06-03 04:00"),
                    },
                    Holidays = new List<Holiday> {
                        FakeHoliday.Create("2020-06-02", "2020-06-02", HolidayType.PublicHoliday),
                        FakeHoliday.Create("2020-06-01", "2020-06-03", HolidayType.Holiday),
                    },
                    Expected = new { TotalTimeWorked = TimeSpan.FromHours(2), PersonalWorkdays = 0, PublicWorkdays = 2 },
                },
                new WorkedDaysOverviewTestCase
                {
                    Identifier = "No_TimeSheets_But_Filtered",
                    Filters = FakeFilters.Create("<2020-06-04", ">=2020-06-02"),
                    Expected = new { TotalTimeWorked = TimeSpan.FromHours(0), PersonalWorkdays = 2, PublicWorkdays = 2 },
                },
                new WorkedDaysOverviewTestCase
                {
                    Identifier = "Cut_First_By_Filter",
                    MasterData = masterData,
                    TimeSheets = new List<TimeSheet> {
                        CreateTimeSheet(customer, activity, "2020-06-01 03:00", "2020-06-01 04:00"),
                        CreateTimeSheet(customer, activity, "2020-06-03 03:00", "2020-06-03 04:00"),
                    },
                    Filters = FakeFilters.Create("<2020-06-04", ">=2020-06-02"),
                    Expected = new { TotalTimeWorked = TimeSpan.FromHours(1), PersonalWorkdays = 2, PublicWorkdays = 2 },
                },
                new WorkedDaysOverviewTestCase
                {
                    Identifier = "Cut_Last_By_Filter",
                    MasterData = masterData,
                    TimeSheets = new List<TimeSheet> {
                        CreateTimeSheet(customer, activity, "2020-06-01 03:00", "2020-06-01 04:00"),
                        CreateTimeSheet(customer, activity, "2020-06-03 03:00", "2020-06-03 04:00"),
                    },
                    Filters = FakeFilters.Create("<2020-06-03", ">=2020-06-01"),
                    Expected = new { TotalTimeWorked = TimeSpan.FromHours(1), PersonalWorkdays = 2, PublicWorkdays = 2 },
                },
                new WorkedDaysOverviewTestCase
                {
                    Identifier = "Before_start_of_daylight_saving_time",
                    MasterData = masterData,
                    TimeSheets = new List<TimeSheet> { CreateTimeSheet(customer, activity, "2020-03-28 00:30", "2020-03-28 03:30") },
                    Expected = new { TotalTimeWorked = TimeSpan.FromHours(3), PersonalWorkdays = 0, PublicWorkdays = 0 },
                },
                new WorkedDaysOverviewTestCase
                {
                    Identifier = "While_start_of_daylight_saving_time",
                    MasterData = masterData,
                    TimeSheets = new List<TimeSheet> { CreateTimeSheet(customer, activity, "2020-03-29 00:30", "2020-03-29 03:30") },
                    Expected = new { TotalTimeWorked = TimeSpan.FromHours(2), PersonalWorkdays = 0, PublicWorkdays = 0 },
                },
                new WorkedDaysOverviewTestCase
                {
                    Identifier = "While_end_of_daylight_saving_time",
                    MasterData = masterData,
                    TimeSheets = new List<TimeSheet> { CreateTimeSheet(customer, activity, "2020-10-25 00:30", "2020-10-25 03:30") },
                    Expected = new { TotalTimeWorked = TimeSpan.FromHours(4), PersonalWorkdays = 0, PublicWorkdays = 0 },
                },
            };
        }

        private static TimeSheet CreateTimeSheet(Customer customer, Activity activity, string startDate, string endDate)
            => FakeTimeSheet.Create(customer.Id, activity.Id, null, null, FakeDateTime.Offset(startDate), FakeDateTime.Offset(endDate));
    }

    public class WorkedDaysOverviewTestCase : TestCase
    {
        public List<IIdEntityModel> MasterData { get; set; } = new();
        public List<TimeSheet> TimeSheets { get; set; } = new();
        public List<Holiday> Holidays { get; set; } = new();
        public TimeSheetFilterSet Filters { get; set; } = FakeFilters.Empty();
        public object Expected { get; set; } = new();
    }
}