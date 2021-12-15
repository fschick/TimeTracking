using FluentAssertions;
using FS.TimeTracking.Api.REST.Controllers.MasterData;
using FS.TimeTracking.Api.REST.Controllers.Report;
using FS.TimeTracking.Api.REST.Controllers.TimeTracking;
using FS.TimeTracking.Shared.DTOs.Report;
using FS.TimeTracking.Shared.Models.Configuration;
using FS.TimeTracking.Shared.Tests.Services;
using FS.TimeTracking.Tests.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace FS.TimeTracking.Tests.IntegrationTests
{
    [TestClass, ExcludeFromCodeCoverage]
    public class CustomDbFunctionTests
    {
        [DataTestMethod, TestDatabases]
        public async Task WhenDbFunctionDiffSecondsIsUsed_ItWillBeTranslated(DatabaseConfiguration configuration)
        {
            // Prepare
            await using var testHost = await TestHost.Create(configuration);

            var newCustomer = FakeEntityFactory.CreateCustomerDto(hidden: true);
            var createdCustomer = await testHost.Post((CustomerController x) => x.Create(default), newCustomer);

            var newProject = FakeEntityFactory.CreateProjectDto(newCustomer.Id, hidden: true);
            var createdProject = await testHost.Post((ProjectController x) => x.Create(default), newProject);

            var newActivity = FakeEntityFactory.CreateActivityDto(hidden: true);
            var createdActivity = await testHost.Post((ActivityController x) => x.Create(default), newActivity);

            var newTimeSheet = FakeEntityFactory.CreateTimeSheetDto(newProject.Id, newActivity.Id);
            var createdTimeSheet = await testHost.Post((TimeSheetController x) => x.Create(default), newTimeSheet);

            // Act
            var readTimeSheet = await testHost.Get<ReportController, List<WorkTimeDto>>(x => x.GetWorkTimesPerCustomer(default, default, default, default, default, default, default));

            // Check
            readTimeSheet.Should().HaveCount(1);

            // Cleanup
            await testHost.Delete((TimeSheetController x) => x.Delete(createdCustomer.Id));
            await testHost.Delete((ActivityController x) => x.Delete(createdProject.Id));
            await testHost.Delete((ProjectController x) => x.Delete(createdActivity.Id));
            await testHost.Delete((CustomerController x) => x.Delete(createdTimeSheet.Id));
        }
    }
}
