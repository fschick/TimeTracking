using FluentAssertions;
using FS.TimeTracking.Api.REST.Controllers.Report;
using FS.TimeTracking.Shared.DTOs.Report;
using FS.TimeTracking.Shared.Models.Configuration;
using FS.TimeTracking.Tests.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace FS.TimeTracking.Tests.IntegrationTests;

[TestClass, ExcludeFromCodeCoverage]
public class ComplexSqlOperationTests
{
    [DataTestMethod, TestDatabases]
    public async Task WhenReportWorkedTimesPerOrderIsQueried_SqlGetsTranslatedAndNoExceptionIsThrown(DatabaseConfiguration configuration)
    {
        // Prepare
        await using var testHost = await TestHost.Create(configuration);

        // Act
        var workTimesPerOrder = await testHost.Get<ReportController, List<WorkTimeDto>>(x => x.GetWorkTimesPerOrder(default, default, default, default, default, default, default));

        // Check
        workTimesPerOrder.Should().NotBeNull();
    }
}