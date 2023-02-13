using FluentAssertions;
using FS.TimeTracking.Abstractions.DTOs.TimeTracking;
using FS.TimeTracking.Api.REST.Controllers.MasterData;
using FS.TimeTracking.Api.REST.Controllers.TimeTracking;
using FS.TimeTracking.Application.Tests.Services;
using FS.TimeTracking.Core.Models.Configuration;
using FS.TimeTracking.Tests.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace FS.TimeTracking.Tests.IntegrationTests;

[TestClass, ExcludeFromCodeCoverage]
public class FilterTests
{
    [DataTestMethod, TestDatabases]
    public async Task WhenTimeSheetOverviewIsRetrievedWithFilter_NoExceptionIsThrown(DatabaseConfiguration configuration)
    {
        // Prepare
        using var faker = new Faker();
        await using var testHost = await TestHost.Create(configuration);

        var newCustomer = faker.Customer.CreateDto(hidden: true);
        var createdCustomer = await testHost.Post((CustomerController x) => x.Create(default), newCustomer);

        var newActivity = faker.Activity.CreateDto(hidden: true);
        var createdActivity = await testHost.Post((ActivityController x) => x.Create(default), newActivity);

        var newProject = faker.Project.CreateDto(newCustomer.Id, hidden: true);
        var createdProject = await testHost.Post((ProjectController x) => x.Create(default), newProject);

        // Act
        var newTimeSheet = faker.TimeSheet.CreateDto(newCustomer.Id, newActivity.Id, newProject.Id);
        var createdTimeSheet = await testHost.Post((TimeSheetController x) => x.Create(default), newTimeSheet);
        var readTimeSheet = await testHost.Get<List<TimeSheetGridDto>>("api/v1/TimeSheet/GetGridFiltered?timeSheetStartDate=2000-01-01_2010-01-01");

        // Check
        readTimeSheet.Should().NotBeNull();

        // Cleanup
        await testHost.Delete((TimeSheetController x) => x.Delete(createdTimeSheet.Id));
        await testHost.Delete((ProjectController x) => x.Delete(createdProject.Id));
        await testHost.Delete((ActivityController x) => x.Delete(createdActivity.Id));
        await testHost.Delete((CustomerController x) => x.Delete(createdCustomer.Id));
    }
}