using Autofac.Extras.FakeItEasy;
using FluentAssertions;
using FS.TimeTracking.Abstractions.Interfaces.Repository.Services;
using FS.TimeTracking.Abstractions.Models.Application.MasterData;
using FS.TimeTracking.Abstractions.Models.Configuration;
using FS.TimeTracking.Repository.DbContexts;
using FS.TimeTracking.Repository.Services;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace FS.TimeTracking.Repository.Tests.Tests;

[TestClass, ExcludeFromCodeCoverage]
public class RepositoryTests
{
    [TestMethod]
    public async Task WhenEntityIsAdded_ThenCreatedAndModifiedDateAreSet()
    {
        // Prepare
        using var autoFake = new AutoFake();
        autoFake.Provide(Options.Create(new TimeTrackingConfiguration { Database = new DatabaseConfiguration { ConnectionString = "Data Source=timetracking.test.sqlite", Type = DatabaseType.Sqlite } }));
        autoFake.Provide<IRepository, Repository<TimeTrackingDbContext>>();

        // Act
        var repository = autoFake.Resolve<IRepository>();
        var customer = new Customer { Id = Guid.NewGuid() };
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
        autoFake.Provide(Options.Create(new TimeTrackingConfiguration { Database = new DatabaseConfiguration { ConnectionString = "Data Source=timetracking.test.sqlite", Type = DatabaseType.Sqlite } }));
        autoFake.Provide<IRepository, Repository<TimeTrackingDbContext>>();

        // Act
        var repository = autoFake.Resolve<IRepository>();
        var customer = new Customer { Id = Guid.NewGuid() };
        var addedCustomer = await repository.Add(customer);
        addedCustomer.Title = "TestName";
        var updatedCustomer = repository.Update(customer);

        // Check
        updatedCustomer.Modified.Should().BeAfter(updatedCustomer.Created);
    }
}