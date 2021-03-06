using Autofac.Extras.FakeItEasy;
using FluentAssertions;
using FS.TimeTracking.Repository.DbContexts;
using FS.TimeTracking.Repository.Services;
using FS.TimeTracking.Shared.Interfaces.Services;
using FS.TimeTracking.Shared.Models.Configuration;
using FS.TimeTracking.Shared.Models.TimeTracking;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace FS.TimeTracking.Repository.Tests.Tests
{
    [TestClass]
    public class RepositoryTests
    {
        [TestMethod]
        public async Task WhenEntityIsAdded_ThenCreatedAndModifiedDateAreSet()
        {
            // Prepare
            using var autoFake = new AutoFake();
            autoFake.Provide(new TimeTrackingConfiguration { Database = new DatabaseConfiguration { ConnectionString = "timetracking" } });
            autoFake.Provide<IRepository, Repository<TimeTrackingDbContext>>();

            // Act
            var repository = autoFake.Resolve<IRepository>();
            var customer = new Customer();
            var addedCustomer = await repository.Add(customer);

            // Check
            addedCustomer.Created.Should().BeAfter(DateTime.UtcNow.Date);
            addedCustomer.Modified.Should().BeAfter(DateTime.UtcNow.Date);
            addedCustomer.Modified.Should().Be(addedCustomer.Created);
        }

        [TestMethod]
        public async Task WhenEntityIsUpdate_ThenModifiedDateIsUpdated()
        {
            // Prepare
            using var autoFake = new AutoFake();
            autoFake.Provide(new TimeTrackingConfiguration { Database = new DatabaseConfiguration { ConnectionString = "timetracking" } });
            autoFake.Provide<IRepository, Repository<TimeTrackingDbContext>>();

            // Act
            var repository = autoFake.Resolve<IRepository>();
            var customer = new Customer();
            var addedCustomer = await repository.Add(customer);
            addedCustomer.ShortName = "TestName";
            var updatedCustomer = repository.Update(customer);

            // Check
            updatedCustomer.Modified.Should().BeAfter(updatedCustomer.Created);
        }
    }
}
