using FluentAssertions;
using FS.TimeTracking.Api.REST.Controllers.MasterData;
using FS.TimeTracking.Application.Tests.Services;
using FS.TimeTracking.Core.Models.Configuration;
using FS.TimeTracking.Tests.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace FS.TimeTracking.Tests.IntegrationTests;

[TestClass, ExcludeFromCodeCoverage]
public class ReferenceExceptionTests
{
    [DataTestMethod, TestDatabases]
    public async Task WhenReferencedEntityShouldRemoved_HttpStatusConflictIsReturned(DatabaseConfiguration configuration)
    {
        // Prepare
        var faker = new Faker(2000);
        await using var testHost = await TestHost.Create(configuration);
        using var client = testHost.GetTestClient();

        // Act
        var newCustomer = faker.Customer.CreateDto();
        var customerCreateRoute = testHost.GetRoute<CustomerController>(x => x.Create(default));
        await client.PostAsJsonAsync(customerCreateRoute, newCustomer);

        var newProject = faker.Project.CreateDto(newCustomer.Id);
        var projectCreateRoute = testHost.GetRoute<ProjectController>(x => x.Create(default));
        await client.PostAsJsonAsync(projectCreateRoute, newProject);

        var customerDeleteRoute = testHost.GetRoute<CustomerController>(x => x.Delete(newCustomer.Id));
        using var response = await client.DeleteAsync(customerDeleteRoute);

        // Check
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);

        // Cleanup
        var projectDeleteRoute = testHost.GetRoute<ProjectController>(x => x.Delete(newProject.Id));
        await client.DeleteAsync(projectDeleteRoute);
        await client.DeleteAsync(customerDeleteRoute);
    }
}