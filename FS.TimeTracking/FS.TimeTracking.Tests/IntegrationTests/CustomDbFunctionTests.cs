using FluentAssertions;
using FluentAssertions.Execution;
using FS.TimeTracking.Abstractions.DTOs.Chart;
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
        await using var testHost = await TestHost.Create(configuration);

        var newCustomer = FakeCustomer.CreateDto(hidden: true);
        var createdCustomer = await testHost.Post((CustomerController x) => x.Create(default), newCustomer);

        var newProject = FakeProject.CreateDto(newCustomer.Id, hidden: true);
        var createdProject = await testHost.Post((ProjectController x) => x.Create(default), newProject);

        var newActivity = FakeActivity.CreateDto(hidden: true);
        var createdActivity = await testHost.Post((ActivityController x) => x.Create(default), newActivity);

        var newTimeSheet = FakeTimeSheet.CreateDto(newProject.Id, newActivity.Id);
        var createdTimeSheet = await testHost.Post((TimeSheetController x) => x.Create(default), newTimeSheet);

        // Act
        var readTimeSheet = await testHost.Get<CustomerChartController, List<CustomerWorkTimeDto>>(x => x.GetWorkTimesPerCustomer(default, default));

        // Check
        using var _ = new AssertionScope();
        readTimeSheet.Should().ContainSingle();
        readTimeSheet.Single().TimeWorked.TotalHours.Should().Be(12);

        // Cleanup
        await testHost.Delete((TimeSheetController x) => x.Delete(createdCustomer.Id));
        await testHost.Delete((ActivityController x) => x.Delete(createdProject.Id));
        await testHost.Delete((ProjectController x) => x.Delete(createdActivity.Id));
        await testHost.Delete((CustomerController x) => x.Delete(createdTimeSheet.Id));
    }
}