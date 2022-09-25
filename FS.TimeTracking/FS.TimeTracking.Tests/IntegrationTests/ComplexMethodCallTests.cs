#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
using FluentAssertions;
using FS.TimeTracking.Abstractions.DTOs.Reporting;
using FS.TimeTracking.Api.REST.Controllers.Reporting;
using FS.TimeTracking.Core.Models.Configuration;
using FS.TimeTracking.Tests.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace FS.TimeTracking.Tests.IntegrationTests;

[TestClass, ExcludeFromCodeCoverage]
public class ComplexMethodCallTests
{
    [DataTestMethod, TestDatabases]
    public async Task WhenGetCustomersHavingTimeSheetsIsCalled_NoInvalidOperationExceptionIsThrown(DatabaseConfiguration configuration)
    {
        // Prepare
        await using var testHost = await TestHost.Create(configuration);

        // Act
        var customers = await testHost.Get<ActivityReportController, List<ActivityReportGridDto>>(x => x.GetCustomersHavingTimeSheets(default, default, default));

        // Check
        customers.Should().NotBeNull();
        customers.Should().BeEmpty();
    }
}
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
