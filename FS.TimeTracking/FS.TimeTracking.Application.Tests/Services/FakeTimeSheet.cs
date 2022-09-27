using FS.TimeTracking.Abstractions.DTOs.TimeTracking;
using FS.TimeTracking.Core.Models.Application.MasterData;
using FS.TimeTracking.Core.Models.Application.TimeTracking;
using System;
using System.Diagnostics.CodeAnalysis;

namespace FS.TimeTracking.Application.Tests.Services;

[ExcludeFromCodeCoverage]
public static class FakeTimeSheet
{
    public static TimeSheet Create(Guid projectId, Guid activityId, Guid? orderId = null, DateTimeOffset? startDate = null, DateTimeOffset? endDate = null, string prefix = "Test")
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
        };

    public static TimeSheet Create(Project project, Activity activity, Order order = null, DateTimeOffset? startDate = null, DateTimeOffset? endDate = null, string prefix = "Test")
    {
        var timeSheet = Create(project.Id, activity.Id, order?.Id, startDate, endDate, prefix);
        timeSheet.Project = project;
        timeSheet.Activity = activity;
        timeSheet.Order = order;
        return timeSheet;
    }

    public static TimeSheetDto CreateDto(Guid projectId, Guid activityId, Guid? orderId = null, DateTimeOffset? startDate = null, DateTimeOffset? endDate = null, string prefix = "Test")
        => FakeAutoMapper.Mapper.Map<TimeSheetDto>(Create(projectId, activityId, orderId, startDate, endDate, prefix));
}