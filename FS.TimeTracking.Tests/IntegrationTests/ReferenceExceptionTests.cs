﻿using FluentAssertions;
using FS.TimeTracking.Api.REST.Controllers;
using FS.TimeTracking.Shared.Models.Configuration;
using FS.TimeTracking.Tests.Services;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace FS.TimeTracking.Tests.IntegrationTests
{
    [TestClass]
    public class ReferenceExceptionTests
    {
        [DataTestMethod, TestDatabases]
        public async Task WhenReferencedEntityShouldRemoved_HttpStatusConflictIsReturned(DatabaseConfiguration configuration)
        {
            // Prepare
            await using var testHost = await TestHost.Create(configuration);
            using var client = testHost.GetTestClient();

            // Act
            var newCustomer = FakeEntityFactory.CreateCustomer();
            var customerCreateRoute = testHost.GetRoute<CustomerController>(x => x.Create(default));
            await client.PostAsJsonAsync(customerCreateRoute, newCustomer);

            var newProject = FakeEntityFactory.CreateProject(newCustomer.Id);
            var projectCreateRoute = testHost.GetRoute<ProjectController>(x => x.Create(default));
            await client.PostAsJsonAsync(projectCreateRoute, newProject);

            var customerDeleteRoute = testHost.GetRoute<CustomerController>(x => x.Delete(newCustomer.Id));
            var response = await client.DeleteAsync(customerDeleteRoute);

            // Check
            response.StatusCode.Should().Be(HttpStatusCode.Conflict);

            // Cleanup
            var projectDeleteRoute = testHost.GetRoute<ProjectController>(x => x.Delete(newProject.Id));
            await client.DeleteAsync(projectDeleteRoute);
            await client.DeleteAsync(customerDeleteRoute);
        }
    }
}
