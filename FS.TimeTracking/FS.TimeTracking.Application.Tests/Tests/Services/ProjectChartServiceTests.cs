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
using Microsoft.Extensions.DependencyInjection;
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

        using var faker = new Faker();
        faker.ConfigureInMemoryDatabase();
        faker.Provide<IWorkdayService, WorkdayService>();
        faker.Provide<IProjectChartApiService, ProjectChartService>();

        var dbRepository = faker.GetRequiredService<IDbRepository>();
        await dbRepository.AddRange(testCase.MasterData);
        await dbRepository.AddRange(testCase.TimeSheets);
        await dbRepository.SaveChanges();

        // Act
        var projectChartService = faker.GetRequiredService<IProjectChartApiService>();
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
        using var faker = new Faker();
        var customer = faker.Customer.Create();
        var project1 = faker.Project.Create(customer.Id, "Test1");
        var project2 = faker.Project.Create(customer.Id, "Test2");
        var activity = faker.Activity.Create();
        var masterData = new List<IIdEntityModel> { customer, project1, project2, activity };

        return new List<TestCase>
        {
            new WorkTimesPerProjectTestCase
            {
                Identifier = "TwoProjects2And1Third",
                MasterData = masterData,
                TimeSheets = new List<TimeSheet> {
                    CreateTimeSheet(customer, project1, activity, faker),
                    CreateTimeSheet(customer, project1, activity, faker),
                    CreateTimeSheet(customer, project2, activity, faker),
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

    private static TimeSheet CreateTimeSheet(Customer customer, Project project, Activity activity, Faker faker)
        => faker.TimeSheet.Create(customer.Id, activity.Id, project.Id, null, faker.DateTime.Offset("2020-06-01 03:00"), faker.DateTime.Offset("2020-06-01 04:00"));
}

public class WorkTimesPerProjectTestCase : TestCase
{
    public List<IIdEntityModel> MasterData { get; set; } = new();
    public List<TimeSheet> TimeSheets { get; set; } = new();
    public List<object> Expected { get; set; } = new();
}