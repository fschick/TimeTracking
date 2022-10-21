using Autofac.Extras.FakeItEasy;
using FluentAssertions;
using FluentAssertions.Execution;
using FS.TimeTracking.Abstractions.DTOs.MasterData;
using FS.TimeTracking.Application.Services.Chart;
using FS.TimeTracking.Application.Services.MasterData;
using FS.TimeTracking.Application.Services.Shared;
using FS.TimeTracking.Application.Tests.Attributes;
using FS.TimeTracking.Application.Tests.Extensions;
using FS.TimeTracking.Application.Tests.Models;
using FS.TimeTracking.Application.Tests.Services;
using FS.TimeTracking.Core.Exceptions;
using FS.TimeTracking.Core.Interfaces.Application.Services.Chart;
using FS.TimeTracking.Core.Interfaces.Application.Services.MasterData;
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
    public async Task WhenGetWorkTimesPerActivityRequested_ResultMatchesExpectedValues(string testCaseJson)
    {
        // Prepare
        var testCase = TestCase.FromJson<WorkTimesPerActivityTestCase>(testCaseJson);

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

    [TestMethod]
    public async Task WhenActivityWithProjectIsAddedAndCustomersDoesNotMatch_ConformityExceptionIsThrown()
    {
        // Prepare
        using var autoFake = new AutoFake();
        await autoFake.ConfigureInMemoryDatabase();
        autoFake.Provide(FakeAutoMapper.Mapper);
        autoFake.Provide<IRepository, Repository<TimeTrackingDbContext>>();
        autoFake.Provide<IActivityService, ActivityService>();

        var repository = autoFake.Resolve<IRepository>();
        var activityService = autoFake.Resolve<IActivityService>();

        var projectCustomer = FakeCustomer.Create();
        var project = FakeProject.Create(projectCustomer.Id);

        var activityCustomer = FakeCustomer.Create();

        await repository.AddRange(new List<IIdEntityModel> { activityCustomer, projectCustomer, project });
        await repository.SaveChanges();

        // Act
        var activity = FakeActivity.CreateDto(activityCustomer.Id, project.Id);
        var createActivity = () => activityService.Create(activity);

        // Check
        await createActivity.Should()
            .ThrowAsync<ConformityException>()
            .WithMessage("Customer of activity does not match customer of related project.");
    }

    [TestMethod]
    public async Task WhenActivityWithProjectIsAddedAndProjectHasNoCustomer_NoExceptionIsThrown()
    {
        // Prepare
        using var autoFake = new AutoFake();
        await autoFake.ConfigureInMemoryDatabase();
        autoFake.Provide(FakeAutoMapper.Mapper);
        autoFake.Provide<IRepository, Repository<TimeTrackingDbContext>>();
        autoFake.Provide<IActivityService, ActivityService>();

        var repository = autoFake.Resolve<IRepository>();
        var activityService = autoFake.Resolve<IActivityService>();

        var project = FakeProject.Create();

        var activityCustomer = FakeCustomer.Create();

        await repository.AddRange(new List<IIdEntityModel> { activityCustomer, project });
        await repository.SaveChanges();

        // Act
        var activity = FakeActivity.CreateDto(activityCustomer.Id, project.Id);
        var createActivity = () => activityService.Create(activity);

        // Check
        await createActivity.Should().NotThrowAsync<ConformityException>();
    }


    [TestMethod]
    public async Task WhenActivityWithCustomerIsUpdatedButTimeSheetsWithDifferentCustomerExists_ConformityExceptionIsThrown()
    {
        // Prepare
        using var autoFake = new AutoFake();
        await autoFake.ConfigureInMemoryDatabase();
        autoFake.Provide(FakeAutoMapper.Mapper);
        autoFake.Provide<IRepository, Repository<TimeTrackingDbContext>>();
        autoFake.Provide<IActivityService, ActivityService>();

        var repository = autoFake.Resolve<IRepository>();
        var activityService = autoFake.Resolve<IActivityService>();

        var activity = FakeActivity.Create();
        await repository.AddRange(new List<IIdEntityModel> { activity });
        await repository.SaveChanges();

        var timeSheetCustomer = FakeCustomer.Create();
        var timeSheet = FakeTimeSheet.Create(timeSheetCustomer.Id, activity.Id);

        await repository.AddRange(new List<IIdEntityModel> { timeSheetCustomer, timeSheet });
        await repository.SaveChanges();

        // Act
        var activityDto = FakeAutoMapper.Mapper.Map<ActivityDto>(activity);
        activityDto.CustomerId = Guid.NewGuid();
        var updateActivity = () => activityService.Update(activityDto);

        // Check
        await updateActivity.Should()
            .ThrowAsync<ConformityException>()
            .WithMessage("Activity is already assigned to different customers via time sheets.");
    }

    [TestMethod]
    public async Task WhenActivityWithProjectIsUpdatedButTimeSheetsWithDifferentProjectsExists_ConformityExceptionIsThrown()
    {
        // Prepare
        using var autoFake = new AutoFake();
        await autoFake.ConfigureInMemoryDatabase();
        autoFake.Provide(FakeAutoMapper.Mapper);
        autoFake.Provide<IRepository, Repository<TimeTrackingDbContext>>();
        autoFake.Provide<IActivityService, ActivityService>();

        var repository = autoFake.Resolve<IRepository>();
        var activityService = autoFake.Resolve<IActivityService>();

        var activity = FakeActivity.Create();
        await repository.AddRange(new List<IIdEntityModel> { activity });
        await repository.SaveChanges();

        var timeSheetCustomer = FakeCustomer.Create();
        var timeSheetProject = FakeProject.Create();
        var timeSheet = FakeTimeSheet.Create(timeSheetCustomer.Id, activity.Id, timeSheetProject.Id);

        await repository.AddRange(new List<IIdEntityModel> { timeSheetCustomer, timeSheetProject, timeSheet });
        await repository.SaveChanges();

        // Act
        var activityDto = FakeAutoMapper.Mapper.Map<ActivityDto>(activity);
        activityDto.ProjectId = Guid.NewGuid();
        var updateActivity = () => activityService.Update(activityDto);

        // Check
        await updateActivity.Should()
            .ThrowAsync<ConformityException>()
            .WithMessage("Activity is already assigned to different projects via time sheets.");
    }
}

public class WorkTimesPerActivityDataSourceAttribute : TestCaseDataSourceAttribute
{
    public WorkTimesPerActivityDataSourceAttribute() : base(GetTestCases()) { }

    private static List<TestCase> GetTestCases()
    {
        var customer = FakeCustomer.Create();
        var activity1 = FakeActivity.Create(prefix: "Test1");
        var activity2 = FakeActivity.Create(prefix: "Test2");
        var masterData = new List<IIdEntityModel> { customer, activity1, activity2 };

        return new List<TestCase>
        {
            new WorkTimesPerActivityTestCase
            {
                Identifier = "TwoActivities2And1Third",
                MasterData = masterData,
                TimeSheets = new List<TimeSheet> {
                    CreateTimeSheet(customer, activity1),
                    CreateTimeSheet(customer, activity1),
                    CreateTimeSheet(customer, activity2),
                },
                Expected = new List<object>
                {
                    new { ActivityTitle = "Test1Activity", TimeWorked = TimeSpan.FromHours(2) },
                    new { ActivityTitle = "Test2Activity", TimeWorked = TimeSpan.FromHours(1) },
                },
            },
            new WorkTimesPerActivityTestCase
            {
                Identifier = "NoTimeSheets",
                Expected = new List<object>(),
            },
        };
    }

    private static TimeSheet CreateTimeSheet(Customer customer, Activity activity)
        => FakeTimeSheet.Create(customer.Id, activity.Id, null, null, FakeDateTime.Offset("2020-06-01 03:00"), FakeDateTime.Offset("2020-06-01 04:00"));
}

public class WorkTimesPerActivityTestCase : TestCase
{
    public List<IIdEntityModel> MasterData { get; set; } = new();
    public List<TimeSheet> TimeSheets { get; set; } = new();
    public List<object> Expected { get; set; } = new();
}