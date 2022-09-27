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
using FS.TimeTracking.Core.Interfaces.Application.Services.Shared;
using FS.TimeTracking.Core.Interfaces.Application.Services.TimeTracking;
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
using System.Text.Json;
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

    [DataTestMethod]
    [WorkedDaysOverviewDataSource]
    public async Task WhenWorkedDaysOverviewRequested_ResultMatchesExpectedValues(WorkedDaysOverviewTestCase testCase)
    {
        // Prepare
        using var autoFake = new AutoFake();

        await autoFake.ConfigureInMemoryDatabase();
        autoFake.Provide(FakeAutoMapper.Mapper);
        autoFake.Provide<IRepository, Repository<TimeTrackingDbContext>>();
        autoFake.Provide<IWorkdayService, WorkdayService>();
        autoFake.Provide<ITimeSheetService, TimeSheetService>();

        var repository = autoFake.Resolve<IRepository>();
        await repository.AddRange(testCase.Holidays);
        await repository.AddRange(testCase.TimeSheets.EliminateDuplicateReferences());
        await repository.SaveChanges();

        // Act
        var timeSheetService = autoFake.Resolve<ITimeSheetService>();
        var filters = JsonSerializer.Deserialize<TimeSheetFilterSet>(testCase.Filters);
        var workDayOverview = await timeSheetService.GetWorkedDaysOverview(filters);

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
            var project = FakeProject.Create(customer);
            var activity = FakeActivity.Create(project);

            return new List<TestCase>
            {
                new WorkedDaysOverviewTestCase
                {
                    Identifier = "No_PublicHolidays_No_Vacation",
                    TimeSheets = new List<TimeSheet> { CreateTimeSheet(project, activity, "2020-06-01 03:00", "2020-06-01 04:00") },
                    Expected = new { TotalTimeWorked = TimeSpan.FromHours(1), PersonalWorkdays = 1, PublicWorkdays = 1 },
                },
                new WorkedDaysOverviewTestCase
                {
                    Identifier = "No_PublicHolidays_But_Vacation",
                    TimeSheets = new List<TimeSheet> {
                        CreateTimeSheet(project, activity, "2020-06-01 03:00", "2020-06-01 04:00"),
                        CreateTimeSheet(project, activity, "2020-06-03 03:00", "2020-06-03 04:00"),
                    },
                    Holidays = new List<Holiday> { FakeHoliday.Create("2020-06-02", "2020-06-02", HolidayType.Holiday)  },
                    Expected = new { TotalTimeWorked = TimeSpan.FromHours(2), PersonalWorkdays = 2, PublicWorkdays = 3 },
                },
                new WorkedDaysOverviewTestCase
                {
                    Identifier = "PublicHolidays_But_No_Vacation",
                    TimeSheets = new List<TimeSheet> {
                        CreateTimeSheet(project, activity, "2020-06-01 03:00", "2020-06-01 04:00"),
                        CreateTimeSheet(project, activity, "2020-06-03 03:00", "2020-06-03 04:00"),
                    },
                    Holidays = new List<Holiday> { FakeHoliday.Create("2020-06-02", "2020-06-02", HolidayType.PublicHoliday)  },
                    Expected = new { TotalTimeWorked = TimeSpan.FromHours(2), PersonalWorkdays = 2, PublicWorkdays = 2 },
                },
                new WorkedDaysOverviewTestCase
                {
                    Identifier = "PublicHolidays_SameDay_Vacation",
                    TimeSheets = new List<TimeSheet> {
                        CreateTimeSheet(project, activity, "2020-06-01 03:00", "2020-06-01 04:00"),
                        CreateTimeSheet(project, activity, "2020-06-03 03:00", "2020-06-03 04:00"),
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
                    TimeSheets = new List<TimeSheet> {
                        CreateTimeSheet(project, activity, "2020-06-01 03:00", "2020-06-01 04:00"),
                        CreateTimeSheet(project, activity, "2020-06-03 03:00", "2020-06-03 04:00"),
                    },
                    Holidays = new List<Holiday> {
                        FakeHoliday.Create("2020-06-02", "2020-06-02", HolidayType.PublicHoliday),
                        FakeHoliday.Create("2020-06-01", "2020-06-03", HolidayType.Holiday),
                    },
                    Expected = new { TotalTimeWorked = TimeSpan.FromHours(2), PersonalWorkdays = 0, PublicWorkdays = 2 },
                },
                new WorkedDaysOverviewTestCase
                {
                    Identifier = "No_TimeSheets",
                    Expected = new { TotalTimeWorked = TimeSpan.FromHours(0), PersonalWorkdays = 1, PublicWorkdays = 1 },
                },
                new WorkedDaysOverviewTestCase
                {
                    Identifier = "No_TimeSheets_But_Filtered",
                    Filters = JsonSerializer.Serialize(FakeFilters.Create("<2020-06-04", ">=2020-06-02")),
                    Expected = new { TotalTimeWorked = TimeSpan.FromHours(0), PersonalWorkdays = 2, PublicWorkdays = 2 },
                },
                new WorkedDaysOverviewTestCase
                {
                    Identifier = "Cut_First_By_Filter",
                    TimeSheets = new List<TimeSheet> {
                        CreateTimeSheet(project, activity, "2020-06-01 03:00", "2020-06-01 04:00"),
                        CreateTimeSheet(project, activity, "2020-06-03 03:00", "2020-06-03 04:00"),
                    },
                    Filters = JsonSerializer.Serialize(FakeFilters.Create("<2020-06-04", ">=2020-06-02")),
                    Expected = new { TotalTimeWorked = TimeSpan.FromHours(1), PersonalWorkdays = 2, PublicWorkdays = 2 },
                },
                new WorkedDaysOverviewTestCase
                {
                    Identifier = "Cut_Last_By_Filter",
                    TimeSheets = new List<TimeSheet> {
                        CreateTimeSheet(project, activity, "2020-06-01 03:00", "2020-06-01 04:00"),
                        CreateTimeSheet(project, activity, "2020-06-03 03:00", "2020-06-03 04:00"),
                    },
                    Filters = JsonSerializer.Serialize(FakeFilters.Create("<2020-06-03", ">=2020-06-01")),
                    Expected = new { TotalTimeWorked = TimeSpan.FromHours(1), PersonalWorkdays = 2, PublicWorkdays = 2 },
                },
                new WorkedDaysOverviewTestCase
                {
                    Identifier = "Before_start_of_daylight_saving_time",
                    TimeSheets = new List<TimeSheet> { CreateTimeSheet(project, activity, "2020-03-28 00:30", "2020-03-28 03:30") },
                    Expected = new { TotalTimeWorked = TimeSpan.FromHours(3), PersonalWorkdays = 0, PublicWorkdays = 0 },
                },
                new WorkedDaysOverviewTestCase
                {
                    Identifier = "While_start_of_daylight_saving_time",
                    TimeSheets = new List<TimeSheet> { CreateTimeSheet(project, activity, "2020-03-29 00:30", "2020-03-29 03:30") },
                    Expected = new { TotalTimeWorked = TimeSpan.FromHours(2), PersonalWorkdays = 0, PublicWorkdays = 0 },
                },
                new WorkedDaysOverviewTestCase
                {
                    Identifier = "While_end_of_daylight_saving_time",
                    TimeSheets = new List<TimeSheet> { CreateTimeSheet(project, activity, "2020-10-25 00:30", "2020-10-25 03:30") },
                    Expected = new { TotalTimeWorked = TimeSpan.FromHours(4), PersonalWorkdays = 0, PublicWorkdays = 0 },
                },
            };
        }

        private static TimeSheet CreateTimeSheet(Project project, Activity activity, string startDate, string endDate)
            => FakeTimeSheet.Create(project, activity, null, FakeDateTime.Offset(startDate), FakeDateTime.Offset(endDate));
    }

    public class WorkedDaysOverviewTestCase : TestCase
    {
        public List<TimeSheet> TimeSheets { get; set; } = new();
        public List<Holiday> Holidays { get; set; } = new();
        public string Filters { get; set; } = JsonSerializer.Serialize(FakeFilters.Empty());
        public object Expected { get; set; } = new();
    }
}