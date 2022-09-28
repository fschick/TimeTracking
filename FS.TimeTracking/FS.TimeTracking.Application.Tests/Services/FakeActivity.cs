using FS.TimeTracking.Abstractions.DTOs.MasterData;
using FS.TimeTracking.Core.Models.Application.MasterData;
using System;
using System.Diagnostics.CodeAnalysis;

namespace FS.TimeTracking.Application.Tests.Services;

[ExcludeFromCodeCoverage]
public static class FakeActivity
{
    public static Activity Create(Guid? projectId = null, string prefix = "Test", bool hidden = false)
        => new()
        {
            Id = Guid.NewGuid(),
            Title = $"{prefix}{nameof(Activity)}",
            ProjectId = projectId,
            Comment = $"{prefix}{nameof(Activity.Comment)}",
            Hidden = hidden
        };

    public static ActivityDto CreateDto(Guid? projectId = null, string prefix = "Test", bool hidden = false)
        => FakeAutoMapper.Mapper.Map<ActivityDto>(Create(projectId, prefix));
}