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
public class ProjectChartServiceTests
{
    [DataTestMethod]
    [WorkTimesPerProjectDataSource]
    public async Task WhenGetWorkTimesPerProjectRequested_ResultMatchesExpectedValues(string testCaseJson)
    {
        // Prepare
        var testCase = TestCase.FromJson<WorkTimesPerProjectTestCase>(testCaseJson);

        using var autoFake = new AutoFake();
        await autoFake.ConfigureInMemoryDatabase();
        autoFake.Provide(FakeAutoMapper.Mapper);
        autoFake.Provide<IRepository, Repository<TimeTrackingDbContext>>();
        autoFake.Provide<IWorkdayService, WorkdayService>();
        autoFake.Provide<IProjectChartService, ProjectChartService>();

        var repository = autoFake.Resolve<IRepository>();
        await repository.AddRange(testCase.MasterData);
        await repository.AddRange(testCase.TimeSheets);
        await repository.SaveChanges();

        // Act
        var projectChartService = autoFake.Resolve<IProjectChartService>();
        var workTimePerIssue = await projectChartService.GetWorkTimesPerProject(FakeFilters.Empty());

        // Check
        using var _ = new AssertionScope();
        workTimePerIssue.Should().HaveSameCount(testCase.Expected);
        workTimePerIssue.Should().BeEquivalentTo(testCase.Expected, options => options.WithStrictOrdering());
    }
}

public class WorkTimesPerProjectDataSourceAttribute : TestCaseDataSourceAttribute
{
    public WorkTimesPerProjectDataSourceAttribute() : base(GetTestCases()) { }

    private static List<TestCase> GetTestCases()
    {
        var customer = FakeCustomer.Create();
        var project1 = FakeProject.Create(customer.Id, "Test1");
        var project2 = FakeProject.Create(customer.Id, "Test2");
        var activity = FakeActivity.Create();
        var masterData = new List<IIdEntityModel> { customer, project1, project2, activity };

        return new List<TestCase>
        {
            new WorkTimesPerProjectTestCase
            {
                Identifier = "TwoProjects2And1Third",
                MasterData = masterData,
                TimeSheets = new List<TimeSheet> {
                    CreateTimeSheet(customer, project1, activity),
                    CreateTimeSheet(customer, project1, activity),
                    CreateTimeSheet(customer, project2, activity),
                },
                Expected = new List<object>
                {
                    new { ProjectTitle = "Test1Project", TimeWorked = TimeSpan.FromHours(2) },
                    new { ProjectTitle = "Test2Project", TimeWorked = TimeSpan.FromHours(1) },
                },
            },
            new WorkTimesPerProjectTestCase
            {
                Identifier = "NoTimeSheets",
                Expected = new List<object>(),
            },
        };
    }

    private static TimeSheet CreateTimeSheet(Customer customer, Project project, Activity activity)
        => FakeTimeSheet.Create(customer.Id, activity.Id, project.Id, null, FakeDateTime.Offset("2020-06-01 03:00"), FakeDateTime.Offset("2020-06-01 04:00"));
}

public class WorkTimesPerProjectTestCase : TestCase
{
    public List<IIdEntityModel> MasterData { get; set; } = new();
    public List<TimeSheet> TimeSheets { get; set; } = new();
    public List<object> Expected { get; set; } = new();
}