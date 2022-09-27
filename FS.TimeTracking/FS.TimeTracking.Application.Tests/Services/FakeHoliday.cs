using FS.TimeTracking.Abstractions.Enums;
using FS.TimeTracking.Core.Models.Application.MasterData;
using System;
using System.Diagnostics.CodeAnalysis;

namespace FS.TimeTracking.Application.Tests.Services;

[ExcludeFromCodeCoverage]
public static class FakeHoliday
{
    public static Holiday Create(DateTimeOffset startDate, DateTimeOffset endDate, HolidayType type = HolidayType.PublicHoliday, string prefix = "Test")
        => new()
        {
            Id = Guid.NewGuid(),
            Title = $"{prefix}{nameof(Holiday)}",
            StartDate = startDate,
            EndDate = endDate,
            Type = type
        };

    public static Holiday Create(string startDate, string endDate, HolidayType type, string prefix = "Test")
        => Create(FakeDateTime.Offset(startDate), FakeDateTime.Offset(endDate), type, prefix);
}