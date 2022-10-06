﻿#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
using FluentAssertions;
using FS.TimeTracking.Abstractions.DTOs.MasterData;
using FS.TimeTracking.Abstractions.DTOs.TimeTracking;
using FS.TimeTracking.Api.REST.Controllers.MasterData;
using FS.TimeTracking.Api.REST.Controllers.TimeTracking;
using FS.TimeTracking.Application.Tests.Services;
using FS.TimeTracking.Core.Models.Configuration;
using FS.TimeTracking.Tests.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace FS.TimeTracking.Tests.IntegrationTests;

[TestClass, ExcludeFromCodeCoverage]
public class CrudIntegrationTests
{
    [DataTestMethod, TestDatabases]
    public async Task WhenCustomerIsAdded_AllMembersAreSaved(DatabaseConfiguration configuration)
    {
        // Prepare
        await using var testHost = await TestHost.Create(configuration);

        // Act
        var newCustomer = FakeCustomer.CreateDto(hidden: true);
        var createdCustomer = await testHost.Post((CustomerController x) => x.Create(default), newCustomer);
        var readCustomer = await testHost.Get<CustomerController, CustomerDto>(x => x.Get(createdCustomer.Id, default));

        // Check
        createdCustomer.Should().BeEquivalentTo(newCustomer);
        readCustomer.Should().BeEquivalentTo(createdCustomer);

        // Cleanup
        await testHost.Delete<CustomerController>(x => x.Get(createdCustomer.Id, default));
    }

    [DataTestMethod, TestDatabases]
    public async Task WhenTimeSheetIsAdded_AllMembersAreSaved(DatabaseConfiguration configuration)
    {
        // Prepare
        await using var testHost = await TestHost.Create(configuration);

        var newCustomer = FakeCustomer.CreateDto(hidden: true);
        var createdCustomer = await testHost.Post((CustomerController x) => x.Create(default), newCustomer);

        var newActivity = FakeActivity.CreateDto(hidden: true);
        var createdActivity = await testHost.Post((ActivityController x) => x.Create(default), newActivity);

        var newProject = FakeProject.CreateDto(newCustomer.Id, hidden: true);
        var createdProject = await testHost.Post((ProjectController x) => x.Create(default), newProject);

        //// Act
        var newTimeSheet = FakeTimeSheet.CreateDto(newCustomer.Id, newActivity.Id, newProject.Id);
        var createdTimeSheet = await testHost.Post((TimeSheetController x) => x.Create(default), newTimeSheet);
        var readTimeSheet = await testHost.Get<TimeSheetController, TimeSheetDto>(x => x.Get(createdTimeSheet.Id, default));

        // Check
        createdTimeSheet.Should().BeEquivalentTo(newTimeSheet);
        readTimeSheet.Should().BeEquivalentTo(createdTimeSheet);

        // Cleanup
        await testHost.Delete((TimeSheetController x) => x.Delete(createdCustomer.Id));
        await testHost.Delete((ActivityController x) => x.Delete(createdProject.Id));
        await testHost.Delete((ProjectController x) => x.Delete(createdActivity.Id));
        await testHost.Delete((CustomerController x) => x.Delete(createdTimeSheet.Id));
    }
}
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
