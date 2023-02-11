using FS.TimeTracking.Abstractions.DTOs.TimeTracking;
using FS.TimeTracking.Core.Models.Application.TimeTracking;
using System;
using System.Diagnostics.CodeAnalysis;

namespace FS.TimeTracking.Application.Tests.Services.FakeModels;

[ExcludeFromCodeCoverage]
public class FakeTimeSheet
{
    private readonly Faker _faker;

    public FakeTimeSheet(Faker faker)
        => _faker = faker;

    public TimeSheet Create(Guid customerId, Guid activityId, Guid? projectId = null, Guid? orderId = null, DateTimeOffset? startDate = null, DateTimeOffset? endDate = null, string issue = null, Guid? userId = null, string prefix = "Test")
        => new()
        {
            Id = _faker.Guid.Create(),
            CustomerId = customerId,
            ActivityId = activityId,
            ProjectId = projectId,
            OrderId = orderId,
            StartDate = startDate ?? DateTimeOffset.Now.Date,
            EndDate = endDate ?? (startDate == null ? DateTimeOffset.Now.Date.AddHours(12) : null),
            Billable = true,
            Comment = $"{prefix}{nameof(TimeSheet.Comment)}",
            Issue = issue ?? $"{prefix}{nameof(TimeSheet.Issue)}",
            UserId = userId ?? Guid.Empty,
        };

    public TimeSheetDto CreateDto(Guid customerId, Guid activityId, Guid? projectId = null, Guid? orderId = null, DateTimeOffset? startDate = null, DateTimeOffset? endDate = null, string issue = null, Guid? userId = null, string prefix = "Test")
        => _faker.AutoMapper.Map<TimeSheetDto>(Create(customerId, activityId, projectId, orderId, startDate, endDate, issue, userId, prefix));
}