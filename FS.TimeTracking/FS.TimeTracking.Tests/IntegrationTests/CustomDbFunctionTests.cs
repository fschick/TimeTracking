using FluentAssertions;
using FluentAssertions.Execution;
using FS.TimeTracking.Abstractions.DTOs.Chart;
using FS.TimeTracking.Abstractions.DTOs.MasterData;
using FS.TimeTracking.Abstractions.DTOs.TimeTracking;
using FS.TimeTracking.Api.REST.Controllers.Chart;
using FS.TimeTracking.Api.REST.Controllers.MasterData;
using FS.TimeTracking.Api.REST.Controllers.TimeTracking;
using FS.TimeTracking.Application.Tests.Services;
using FS.TimeTracking.Core.Models.Configuration;
using FS.TimeTracking.Tests.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace FS.TimeTracking.Tests.IntegrationTests;

[TestClass, ExcludeFromCodeCoverage]
public class CustomDbFunctionTests
{
    [DataTestMethod, TestDatabases]
    public async Task WhenDbFunctionDiffSecondsIsUsed_ItWillBeTranslated(DatabaseConfiguration configuration)
    {
        // Prepare
        using var faker = new Faker();
        await using var testHost = await TestHost.Create(configuration);

        var (customer, activity, project) = await InsertMasterData(testHost);
        var newTimeSheet = faker.TimeSheet.CreateDto(customer.Id, activity.Id, project.Id);
        var createdTimeSheet = await testHost.Post((TimeSheetController x) => x.Create(default), newTimeSheet);

        // Act
        var readTimeSheet = await testHost.Get<CustomerChartController, List<CustomerWorkTimeDto>>(x => x.GetWorkTimesPerCustomer(default, default));

        // Check
        using var _ = new AssertionScope();
        readTimeSheet.Should().ContainSingle();
        readTimeSheet.Single().TimeWorked.TotalHours.Should().Be(12);

        // Cleanup
        await DeleteMasterData(testHost, createdTimeSheet, project, activity, customer);
    }

    [DataTestMethod, TestDatabases]
    public async Task WhenDiffSecondsIsCalledWhileStartOfDaylightSavingTime_ItWillHandleOffsetDifference(DatabaseConfiguration configuration)
    {
        // Prepare
        using var faker = new Faker();
        await using var testHost = await TestHost.Create(configuration);

        var (customer, activity, project) = await InsertMasterData(testHost);
        var newTimeSheet = faker.TimeSheet.CreateDto(customer.Id, activity.Id, project.Id, startDate: faker.DateTime.Offset("2020-03-29 00:30"), endDate: faker.DateTime.Offset("2020-03-29 03:30"));
        var createdTimeSheet = await testHost.Post((TimeSheetController x) => x.Create(default), newTimeSheet);

        // Act
        var readTimeSheet = await testHost.Get<CustomerChartController, List<CustomerWorkTimeDto>>(x => x.GetWorkTimesPerCustomer(default, default));

        // Check
        using var _ = new AssertionScope();
        readTimeSheet.Should().ContainSingle();
        readTimeSheet.Single().TimeWorked.TotalHours.Should().Be(2);

        // Cleanup
        await DeleteMasterData(testHost, createdTimeSheet, project, activity, customer);
    }

    [DataTestMethod, TestDatabases]
    public async Task WhenDiffSecondsIsCalledWhileEndOfDaylightSavingTime_ItWillHandleOffsetDifference(DatabaseConfiguration configuration)
    {
        // Prepare
        using var faker = new Faker();
        await using var testHost = await TestHost.Create(configuration);

        var (customer, activity, project) = await InsertMasterData(testHost);
        var newTimeSheet = faker.TimeSheet.CreateDto(customer.Id, activity.Id, project.Id, startDate: faker.DateTime.Offset("2020-10-25 00:30"), endDate: faker.DateTime.Offset("2020-10-25 03:30"));
        var createdTimeSheet = await testHost.Post((TimeSheetController x) => x.Create(default), newTimeSheet);

        // Act
        var readTimeSheet = await testHost.Get<CustomerChartController, List<CustomerWorkTimeDto>>(x => x.GetWorkTimesPerCustomer(default, default));

        // Check
        using var _ = new AssertionScope();
        readTimeSheet.Should().ContainSingle();
        readTimeSheet.Single().TimeWorked.TotalHours.Should().Be(4);

        // Cleanup
        await DeleteMasterData(testHost, createdTimeSheet, project, activity, customer);
    }

    private static async Task<(CustomerDto Customer, ActivityDto Activity, ProjectDto Project)> InsertMasterData(TestHost testHost)
    {
        using var faker = new Faker();

        var newCustomer = faker.Customer.CreateDto(hidden: true);
        var createdCustomer = await testHost.Post((CustomerController x) => x.Create(default), newCustomer);

        var newActivity = faker.Activity.CreateDto(hidden: true);
        var createdActivity = await testHost.Post((ActivityController x) => x.Create(default), newActivity);

        var newProject = faker.Project.CreateDto(newCustomer.Id, hidden: true);
        var createdProject = await testHost.Post((ProjectController x) => x.Create(default), newProject);

        return (createdCustomer, createdActivity, createdProject);
    }

    private static async Task DeleteMasterData(TestHost testHost, TimeSheetDto timesheet, ProjectDto project, ActivityDto activity, CustomerDto customer)
    {
        await testHost.Delete((TimeSheetController x) => x.Delete(timesheet.Id));
        await testHost.Delete((ProjectController x) => x.Delete(project.Id));
        await testHost.Delete((ActivityController x) => x.Delete(activity.Id));
        await testHost.Delete((CustomerController x) => x.Delete(customer.Id));
    }
}