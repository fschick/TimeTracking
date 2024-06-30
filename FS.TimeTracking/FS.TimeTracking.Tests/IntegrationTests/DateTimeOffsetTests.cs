#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
using FluentAssertions;
using FS.TimeTracking.Abstractions.DTOs.MasterData;
using FS.TimeTracking.Abstractions.DTOs.TimeTracking;
using FS.TimeTracking.Api.REST.Controllers.MasterData;
using FS.TimeTracking.Api.REST.Controllers.TimeTracking;
using FS.TimeTracking.Application.Tests.Services;
using FS.TimeTracking.Core.Models.Configuration;
using FS.TimeTracking.Tests.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace FS.TimeTracking.Tests.IntegrationTests;

[TestClass, ExcludeFromCodeCoverage]
public class DateTimeOffsetTests
{
    [DataTestMethod, TestDatabases]
    public async Task WhenDateTimeOffsetIsSaveAndReadFromDatabase_ValueDoesNotChange(DatabaseConfiguration configuration)
    {
        // Prepare
        using var faker = new Faker();
        await using var testHost = await TestHost.Create(configuration);

        var newCustomer = faker.Customer.CreateDto();
        var newActivity = faker.Activity.CreateDto();
        var newProject = faker.Project.CreateDto(newCustomer.Id);
        var newTimeSheet = faker.TimeSheet.CreateDto(newCustomer.Id, newActivity.Id, newProject.Id);

        // Act
        newTimeSheet.StartDate = new DateTimeOffset(2020, 1, 1, 0, 0, 0, TimeSpan.FromHours(-4));
        var endDate = new DateTime(2020, 1, 1, 6, 0, 0);
        var cetTimezoneOffset = TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard Time").GetUtcOffset(endDate);
        newTimeSheet.EndDate = new DateTimeOffset(endDate, cetTimezoneOffset);

        await testHost.Post<CustomerController, CustomerDto>(x => x.Create(default), newCustomer);
        await testHost.Post<ProjectController, ProjectDto>(x => x.Create(default), newProject);
        await testHost.Post<ActivityController, ActivityDto>(x => x.Create(default), newActivity);
        await testHost.Post<TimeSheetController, TimeSheetDto>(x => x.Create(default), newTimeSheet);

        var readTimeSheet = await testHost.Get<TimeSheetController, TimeSheetDto>(x => x.Get(newTimeSheet.Id, default));

        // Check
        readTimeSheet!.StartDate.Should().Be(newTimeSheet.StartDate);
        readTimeSheet!.EndDate.Should().Be(newTimeSheet.EndDate);
    }
}
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed