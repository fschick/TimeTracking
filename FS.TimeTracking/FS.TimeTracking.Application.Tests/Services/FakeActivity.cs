using FS.TimeTracking.Abstractions.DTOs.MasterData;
using FS.TimeTracking.Core.Models.Application.MasterData;
using System;
using System.Diagnostics.CodeAnalysis;

namespace FS.TimeTracking.Application.Tests.Services;

[ExcludeFromCodeCoverage]
public static class FakeActivity
{
    public static Activity Create(Guid? customerId = null, Guid? projectId = null, string prefix = "Test", bool hidden = false)
        => new()
        {
            Id = Guid.NewGuid(),
            Title = $"{prefix}{nameof(Activity)}",
            CustomerId = customerId,
            ProjectId = projectId,
            Comment = $"{prefix}{nameof(Activity.Comment)}",
            Hidden = hidden
        };

    public static ActivityDto CreateDto(Guid? customerId = null, Guid? projectId = null, string prefix = "Test", bool hidden = false)
        => FakeAutoMapper.Mapper.Map<ActivityDto>(Create(customerId, projectId, prefix));
}