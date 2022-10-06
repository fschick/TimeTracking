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
public class IssueChartServiceTests
{
    [DataTestMethod]
    [WorkTimesPerIssueDataSource]
    public async Task WhenGetWorkTimesPerIssueRequested_ResultMatchesExpectedValues(WorkTimesPerIssueTestCase testCase)
    {
        // Prepare
        using var autoFake = new AutoFake();

        await autoFake.ConfigureInMemoryDatabase();
        autoFake.Provide(FakeAutoMapper.Mapper);
        autoFake.Provide<IRepository, Repository<TimeTrackingDbContext>>();
        autoFake.Provide<IWorkdayService, WorkdayService>();
        autoFake.Provide<IIssueChartService, IssueChartService>();

        var repository = autoFake.Resolve<IRepository>();
        await repository.AddRange(testCase.MasterData);
        await repository.AddRange(testCase.TimeSheets);
        await repository.SaveChanges();

        // Act
        var issueChartService = autoFake.Resolve<IIssueChartService>();
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
        var customer = FakeCustomer.Create();
        var activity = FakeActivity.Create(customer.Id);
        var masterData = new List<IIdEntityModel> { customer, activity };

        return new List<TestCase>
        {
            new WorkTimesPerIssueTestCase
            {
                Identifier = "Two_issues_2_and_1_third",
                MasterData = masterData,
                TimeSheets = new List<TimeSheet> {
                    CreateTimeSheet(customer, activity, "IssueA"),
                    CreateTimeSheet(customer, activity, "IssueA"),
                    CreateTimeSheet(customer, activity, "IssueB"),
                },
                Expected = new List<object>
                {
                    new { Issue = "IssueA", TimeWorked = TimeSpan.FromHours(2) },
                    new { Issue = "IssueB", TimeWorked = TimeSpan.FromHours(1) },
                },
            },
            new WorkTimesPerIssueTestCase
            {
                Identifier = "No_issues",
                MasterData = masterData,
                TimeSheets = new List<TimeSheet> {
                    CreateTimeSheet(customer, activity, string.Empty),
                    CreateTimeSheet(customer, activity, string.Empty),
                },
                Expected = new List<object> {
                    new { Issue = string.Empty, TimeWorked = TimeSpan.FromHours(2) }
                },
            },
            new WorkTimesPerIssueTestCase
            {
                Identifier = "No_time_sheets",
                Expected = new List<object>(),
            },
        };
    }

    private static TimeSheet CreateTimeSheet(Customer customer, Activity activity, string issue)
        => FakeTimeSheet.Create(customer.Id, activity.Id, null, null, FakeDateTime.Offset("2020-06-01 03:00"), FakeDateTime.Offset("2020-06-01 04:00"), issue: issue);
}

public class WorkTimesPerIssueTestCase : TestCase
{
    public List<IIdEntityModel> MasterData { get; set; } = new();
    public List<TimeSheet> TimeSheets { get; set; } = new();
    public List<object> Expected { get; set; } = new();
}