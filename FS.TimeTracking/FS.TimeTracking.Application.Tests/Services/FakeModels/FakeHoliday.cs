using FS.TimeTracking.Abstractions.DTOs.MasterData;
using FS.TimeTracking.Abstractions.Enums;
using FS.TimeTracking.Core.Models.Application.MasterData;
using System;
using System.Diagnostics.CodeAnalysis;

namespace FS.TimeTracking.Application.Tests.Services.FakeModels;

[ExcludeFromCodeCoverage]
public class FakeHoliday
{
    private readonly Faker _faker;

    public FakeHoliday(Faker faker)
        => _faker = faker;

    public Holiday Create(DateTimeOffset startDate, DateTimeOffset endDate, HolidayType type = HolidayType.PublicHoliday, Guid? userId = null, string prefix = "Test")
    {
        userId = type == HolidayType.Holiday ?
            userId ?? Guid.Empty
            : Guid.Empty;

        return new()
        {
            Id = _faker.Guid.Create(),
            Title = $"{prefix}{nameof(Holiday)}",
            StartDate = startDate,
            EndDate = endDate,
            Type = type,
            UserId = (Guid)userId,
        };
    }

    public Holiday Create(string startDate, string endDate, HolidayType type, Guid? userId = null, string prefix = "Test")
        => Create(_faker.DateTime.Offset(startDate), _faker.DateTime.Offset(endDate), type, userId, prefix);

    public HolidayDto CreateDto(string startDate, string endDate, HolidayType type, Guid? userId = null, string prefix = "Test")
        => _faker.AutoMapper.Map<HolidayDto>(Create(_faker.DateTime.Offset(startDate), _faker.DateTime.Offset(endDate), type, userId, prefix));
}