#pragma warning disable CS4014
using FluentAssertions;
using FS.TimeTracking.Abstractions.DTOs.MasterData;
using FS.TimeTracking.Api.REST.Controllers.MasterData;
using FS.TimeTracking.Application.Tests.Extensions;
using FS.TimeTracking.Application.Tests.Services;
using FS.TimeTracking.Core.Models.Configuration;
using FS.TimeTracking.Tests.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace FS.TimeTracking.Tests.IntegrationTests;

[TestClass, ExcludeFromCodeCoverage]
[SuppressMessage("IDisposableAnalyzers.Correctness", "IDISP013:Await in using", Justification = "False positive")]
public class ReferenceExceptionTests
{
    [DataTestMethod, TestDatabases]
    [SuppressMessage("ReSharper", "AccessToDisposedClosure")]
    public async Task WhenReferencedEntityShouldRemoved_HttpStatusConflictIsReturned(DatabaseConfiguration configuration)
    {
        // Prepare
        using var faker = new Faker();
        await using var testHost = await TestHost.Create(configuration);

        // Act
        var newCustomer = faker.Customer.CreateDto();
        await testHost.Post<CustomerController, CustomerDto>(x => x.Create(default), newCustomer);

        var newProject = faker.Project.CreateDto(newCustomer.Id);
        await testHost.Post<ProjectController, ProjectDto>(x => x.Create(default), newProject);

        var deleteCustomer = () => testHost.Delete<CustomerController>(x => x.Delete(newCustomer.Id));

        // Check
        await deleteCustomer.Should().ThrowConflictAsync();

        // Cleanup
        await testHost.Delete<ProjectController>(x => x.Delete(newProject.Id));
        await testHost.Delete<CustomerController>(x => x.Delete(newCustomer.Id));
    }
}