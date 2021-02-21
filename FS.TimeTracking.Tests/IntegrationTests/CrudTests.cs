using FluentAssertions;
using FS.TimeTracking.Api.REST.Controllers;
using FS.TimeTracking.Shared.DTOs.TimeTracking;
using FS.TimeTracking.Shared.Models.Configuration;
using FS.TimeTracking.Tests.Services;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace FS.TimeTracking.Tests.IntegrationTests
{
    [TestClass]
    public class CrudIntegrationTests
    {
        [TestMethod]
        [TestDatabaseSources]
        public async Task WhenCustomerIsAdded_AllMembersAreSaved(DatabaseConfiguration configuration)
        {
            // Prepare
            await using var testHost = await TimeTrackingTestHost.Create(configuration);
            using var client = testHost.GetTestClient();

            var newCustomer = new CustomerDto
            {
                Id = Guid.NewGuid(),
                ShortName = "TestCustomer",
                CompanyName = "TestCompany",
                ContactName = "TestContact",
                Street = "TestStreet",
                ZipCode = "1234",
                City = "TestCity",
                Country = "TestCountry",
                Hidden = true,
            };

            // Act
            var createRoute = testHost.GetRoute<CustomerController>(x => x.Create(default));
            var response = await client.PostAsJsonAsync(createRoute, newCustomer);
            var createdCustomer = await response.Content.ReadFromJsonAsync<CustomerDto>();

            var getRoute = testHost.GetRoute<CustomerController>(x => x.Get(createdCustomer.Id, default));
            var readCustomer = await client.GetFromJsonAsync<CustomerDto>(getRoute);

            // Check
            createdCustomer.Should().BeEquivalentTo(newCustomer);
            readCustomer.Should().BeEquivalentTo(createdCustomer);

            // Cleanup
            var deleteRoute = testHost.GetRoute<CustomerController>(x => x.Delete(readCustomer.Id));
            await client.DeleteAsync(deleteRoute);
        }
    }
}
