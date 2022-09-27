using Autofac.Extras.FakeItEasy;
using FakeItEasy;
using FluentAssertions;
using FS.TimeTracking.Abstractions.DTOs.MasterData;
using FS.TimeTracking.Abstractions.Enums;
using FS.TimeTracking.Application.Services.Shared;
using FS.TimeTracking.Core.Interfaces.Application.Services.Shared;
using FS.TimeTracking.Core.Interfaces.Repository.Services;
using FS.TimeTracking.Core.Models.Application.MasterData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace FS.TimeTracking.Application.Tests.Tests.Services;

[TestClass, ExcludeFromCodeCoverage]
public class WorkdayServiceTests
{
    private readonly List<HolidayDto> _holidays = new()
    {
        new() { StartDate = new DateTime(2000, 01, 01), EndDate = new DateTime(2000, 01, 01), Title = "New Year", Type = HolidayType.PublicHoliday },
        new() { StartDate = new DateTime(2000, 01, 06), EndDate = new DateTime(2000, 01, 06), Title = "Epiphany", Type = HolidayType.PublicHoliday },

        new() { StartDate = new DateTime(2003, 01, 01), EndDate = new DateTime(2003, 01, 01), Title = "New Year", Type = HolidayType.PublicHoliday },
        new() { StartDate = new DateTime(2003, 01, 06), EndDate = new DateTime(2003, 01, 06), Title = "Epiphany", Type = HolidayType.PublicHoliday },
        new() { StartDate = new DateTime(2002, 12, 30), EndDate = new DateTime(2003, 01, 04), Title = "Vacation", Type = HolidayType.Holiday },
    };

    [DataTestMethod]
    [DynamicData(nameof(GetWorkDayRanges))]
    public async Task WhenWorkdaysRequested_PublicAndPersonalWorkdaysMatchExpected(DateTime start, DateTime end, int publicWorkdays, int personalWorkdays)
    {
        using var autoFake = new AutoFake();

        var repository = autoFake.Resolve<IRepository>();
        A.CallTo(() => repository.Get<Holiday, HolidayDto>(default, default, default, default, default, default, default, default))
            .WithAnyArguments()
            .Returns(Task.FromResult(_holidays));

        autoFake.Provide(repository);
        autoFake.Provide<IWorkdayService, WorkdayService>();
        autoFake.Provide<IWorkdayService, WorkdayService>();

        var workdayService = autoFake.Resolve<IWorkdayService>();
        var workDays = await workdayService.GetWorkdays(start, end);
        workDays.PublicWorkdays.Should().HaveCount(publicWorkdays);
        workDays.PersonalWorkdays.Should().HaveCount(personalWorkdays);
        await Task.Delay(0);
    }

    public static IEnumerable<object[]> GetWorkDayRanges =>
        new List<object[]>
        {
            new object[] { new DateTime(2000, 01, 01), new DateTime(2000, 01, 01), 00, 00 }, // Saturday
            new object[] { new DateTime(2000, 01, 01), new DateTime(2000, 01, 02), 00, 00 }, // Saturday, Sunday
            new object[] { new DateTime(2000, 01, 01), new DateTime(2000, 01, 03), 01, 01 }, // Saturday, Sunday, Monday
            new object[] { new DateTime(2000, 01, 01), new DateTime(2000, 01, 31), 20, 20 },

            new object[] { new DateTime(2003, 01, 01), new DateTime(2003, 01, 01), 00, 00 }, // Wednesday (public holiday)
            new object[] { new DateTime(2003, 01, 01), new DateTime(2003, 01, 02), 01, 00 }, // Wednesday (public holiday), Thursday (vacation)
        };
}