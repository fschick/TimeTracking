using FS.TimeTracking.Abstractions.DTOs.TimeTracking;
using FS.TimeTracking.Core.Models.Application.TimeTracking;
using System;
using System.Diagnostics.CodeAnalysis;

namespace FS.TimeTracking.Application.Tests.Services;

[ExcludeFromCodeCoverage]
public static class FakeTimeSheet
{
    public static TimeSheet Create(Guid projectId, Guid activityId, Guid? orderId = null, DateTimeOffset? startDate = null, DateTimeOffset? endDate = null, string issue = null, string prefix = "Test")
        => new()
        {
            Id = Guid.NewGuid(),
            ProjectId = projectId,
            ActivityId = activityId,
            OrderId = orderId,
            StartDate = startDate ?? DateTimeOffset.Now.Date,
            EndDate = endDate ?? (startDate == null ? DateTimeOffset.Now.Date.AddHours(12) : null),
            Billable = true,
            Comment = $"{prefix}{nameof(TimeSheet.Comment)}",
            Issue = issue ?? $"{prefix}{nameof(TimeSheet.Issue)}",
        };

    public static TimeSheetDto CreateDto(Guid projectId, Guid activityId, Guid? orderId = null, DateTimeOffset? startDate = null, DateTimeOffset? endDate = null, string prefix = "Test")
        => FakeAutoMapper.Mapper.Map<TimeSheetDto>(Create(projectId, activityId, orderId, startDate, endDate, prefix));
}