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
public class IssueChartServiceTests
{
    [DataTestMethod]
    [WorkTimesPerIssueDataSource]
    public async Task WhenGetWorkTimesPerIssueRequested_ResultMatchesExpectedValues(string testCaseJson)
    {
        // Prepare
        var testCase = TestCase.FromJson<WorkTimesPerIssueTestCase>(testCaseJson);

        using var faker = new Faker();
        faker.ConfigureInMemoryDatabase();
        faker.Provide<IWorkdayService, WorkdayService>();
        faker.Provide<IIssueChartApiService, IssueChartService>();

        var dbRepository = faker.GetRequiredService<IDbRepository>();
        await dbRepository.AddRange(testCase.MasterData);
        await dbRepository.AddRange(testCase.TimeSheets);
        await dbRepository.SaveChanges();

        // Act
        var issueChartService = faker.GetRequiredService<IIssueChartApiService>();
        var workTimePerIssue = await issueChartService.GetWorkTimesPerIssue(FakeFilters.Empty());

        // Check
        using var _ = new AssertionScope();
        workTimePerIssue.Should().HaveSameCount(testCase.Expected);
        workTimePerIssue.Should().BeEquivalentTo(testCase.Expected, options => options.WithStrictOrdering());
    }
}

public class WorkTimesPerIssueDataSourceAttribute : TestCaseDataSourceAttribute
{
    public WorkTimesPerIssueDataSourceAttribute() : base(GetTestCases()) { }

    private static List<TestCase> GetTestCases()
    {
        using var faker = new Faker();
        var customer = faker.Customer.Create();
        var activity = faker.Activity.Create(customer.Id);
        var masterData = new List<IIdEntityModel> { customer, activity };

        return new List<TestCase>
        {
            new WorkTimesPerIssueTestCase
            {
                Identifier = "TwoIssues2And1Third",
                MasterData = masterData,
                TimeSheets = new List<TimeSheet> {
                    CreateTimeSheet(customer, activity, "IssueA", faker),
                    CreateTimeSheet(customer, activity, "IssueA", faker),
                    CreateTimeSheet(customer, activity, "IssueB", faker),
                },
                Expected = new List<object>
                {
                    new { Issue = "IssueA", TimeWorked = TimeSpan.FromHours(2) },
                    new { Issue = "IssueB", TimeWorked = TimeSpan.FromHours(1) },
                },
            },
            new WorkTimesPerIssueTestCase
            {
                Identifier = "NoIssues",
                MasterData = masterData,
                TimeSheets = new List<TimeSheet> {
                    CreateTimeSheet(customer, activity, string.Empty, faker),
                    CreateTimeSheet(customer, activity, string.Empty, faker),
                },
                Expected = new List<object> {
                    new { Issue = string.Empty, TimeWorked = TimeSpan.FromHours(2) }
                },
            },
            new WorkTimesPerIssueTestCase
            {
                Identifier = "NoTimeSheets",
                Expected = new List<object>(),
            },
        };
    }

    private static TimeSheet CreateTimeSheet(Customer customer, Activity activity, string issue, Faker faker)
        => faker.TimeSheet.Create(customer.Id, activity.Id, null, null, faker.DateTime.Offset("2020-06-01 03:00"), faker.DateTime.Offset("2020-06-01 04:00"), issue: issue);
}

public class WorkTimesPerIssueTestCase : TestCase
{
    public List<IIdEntityModel> MasterData { get; set; } = new();
    public List<TimeSheet> TimeSheets { get; set; } = new();
    public List<object> Expected { get; set; } = new();
}