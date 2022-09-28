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
using FS.TimeTracking.Repository.DbContexts;
using FS.TimeTracking.Repository.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace FS.TimeTracking.Application.Tests.Tests.Services;

[TestClass, ExcludeFromCodeCoverage]
public class ActivityChartServiceTests
{
    [DataTestMethod]
    [WorkTimesPerActivityDataSource]
    public async Task WhenGetWorkTimesPerActivityRequested_ResultMatchesExpectedValues(WorkTimesPerActivityTestCase testCase)
    {
        // Prepare
        using var autoFake = new AutoFake();

        await autoFake.ConfigureInMemoryDatabase();
        autoFake.Provide(FakeAutoMapper.Mapper);
        autoFake.Provide<IRepository, Repository<TimeTrackingDbContext>>();
        autoFake.Provide<IWorkdayService, WorkdayService>();
        autoFake.Provide<IActivityChartService, ActivityChartService>();

        var repository = autoFake.Resolve<IRepository>();
        await repository.AddRange(testCase.MasterData);
        await repository.AddRange(testCase.TimeSheets);
        await repository.SaveChanges();

        // Act
        var activityChartService = autoFake.Resolve<IActivityChartService>();
        var workTimePerIssue = await activityChartService.GetWorkTimesPerActivity(FakeFilters.Empty());

        // Check
        using var _ = new AssertionScope();
        workTimePerIssue.Should().HaveSameCount(testCase.Expected);
        workTimePerIssue.Should().BeEquivalentTo(testCase.Expected, options => options.WithStrictOrdering());
    }
}

public class WorkTimesPerActivityDataSourceAttribute : TestCaseDataSourceAttribute
{
    public WorkTimesPerActivityDataSourceAttribute() : base(GetTestCases()) { }

    private static List<TestCase> GetTestCases()
    {
        var customer = FakeCustomer.Create();
        var project = FakeProject.Create(customer.Id);
        var activity1 = FakeActivity.Create(project.Id, prefix: "Test1");
        var activity2 = FakeActivity.Create(project.Id, prefix: "Test2");
        var masterData = new List<IIdEntityModel> { customer, project, activity1, activity2 };

        return new List<TestCase>
        {
            new WorkTimesPerActivityTestCase
            {
                Identifier = "Two_activities_2_and_1_third",
                MasterData = masterData,
                TimeSheets = new List<TimeSheet> {
                    CreateTimeSheet(project, activity1),
                    CreateTimeSheet(project, activity1),
                    CreateTimeSheet(project, activity2),
                },
                Expected = new List<object>
                {
                    new { ActivityTitle = "Test1Activity", TimeWorked = TimeSpan.FromHours(2) },
                    new { ActivityTitle = "Test2Activity", TimeWorked = TimeSpan.FromHours(1) },
                },
            },
            new WorkTimesPerActivityTestCase
            {
                Identifier = "No_time_sheets",
                Expected = new List<object>(),
            },
        };
    }

    private static TimeSheet CreateTimeSheet(Project project, Activity activity)
        => FakeTimeSheet.Create(project.Id, activity.Id, null, FakeDateTime.Offset("2020-06-01 03:00"), FakeDateTime.Offset("2020-06-01 04:00"));
}

public class WorkTimesPerActivityTestCase : TestCase
{
    public List<IIdEntityModel> MasterData { get; set; } = new();
    public List<TimeSheet> TimeSheets { get; set; } = new();
    public List<object> Expected { get; set; } = new();
}