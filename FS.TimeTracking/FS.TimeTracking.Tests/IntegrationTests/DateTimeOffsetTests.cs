﻿using FluentAssertions;
using FS.TimeTracking.Api.REST.Controllers;
using FS.TimeTracking.Shared.DTOs.TimeTracking;
using FS.TimeTracking.Shared.Models.Configuration;
using FS.TimeTracking.Shared.Tests.Services;
using FS.TimeTracking.Tests.Services;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace FS.TimeTracking.Tests.IntegrationTests
{
    [TestClass]
    public class DateTimeOffsetTests
    {
        [DataTestMethod, TestDatabases]
        public async Task WhenDateTimeOffsetIsSaveAndReadFromDatabase_ValueDoesNotChange(DatabaseConfiguration configuration)
        {
            // Prepare
            await using var testHost = await TestHost.Create(configuration);
            using var client = testHost.GetTestClient();

            var newCustomer = FakeEntityFactory.CreateCustomer();
            var newProject = FakeEntityFactory.CreateProject(newCustomer.Id);
            var newActivity = FakeEntityFactory.CreateActivity();
            var newTimeSheet = FakeEntityFactory.CreateTimeSheet(newProject.Id, newActivity.Id);

            // Act
            newTimeSheet.StartDate = new DateTimeOffset(2020, 1, 1, 0, 0, 0, TimeSpan.FromHours(-4));
            var endDate = new DateTime(2020, 1, 1, 6, 0, 0);
            var cetTimezoneOffset = TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard Time").GetUtcOffset(endDate);
            newTimeSheet.EndDate = new DateTimeOffset(endDate, cetTimezoneOffset);

            var customerCreateRoute = testHost.GetRoute<CustomerController>(x => x.Create(default));
            await client.PostAsJsonAsync(customerCreateRoute, newCustomer);

            var projectCreateRoute = testHost.GetRoute<ProjectController>(x => x.Create(default));
            await client.PostAsJsonAsync(projectCreateRoute, newProject);

            var activityCreateRoute = testHost.GetRoute<ActivityController>(x => x.Create(default));
            await client.PostAsJsonAsync(activityCreateRoute, newActivity);

            var timeSheetCreateRoute = testHost.GetRoute<TimeSheetController>(x => x.Create(default));
            await client.PostAsJsonAsync(timeSheetCreateRoute, newTimeSheet);

            var getTimeSheetRoute = testHost.GetRoute<TimeSheetController>(x => x.Get(newTimeSheet.Id, default));
            var readTimeSheet = await client.GetFromJsonAsync<TimeSheetDto>(getTimeSheetRoute);

            // Check
            readTimeSheet!.StartDate.Should().Be(newTimeSheet.StartDate);
            readTimeSheet!.EndDate.Should().Be(newTimeSheet.EndDate);
        }
    }
}
