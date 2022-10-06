using FS.TimeTracking.Abstractions.DTOs.MasterData;
using FS.TimeTracking.Core.Models.Application.MasterData;
using System;
using System.Diagnostics.CodeAnalysis;

namespace FS.TimeTracking.Application.Tests.Services;

[ExcludeFromCodeCoverage]
public static partial class FakeProject
{
    public static Project Create(Guid? customerId = null, string prefix = "Test", bool hidden = false)
        => new()
        {
            Id = Guid.NewGuid(),
            Title = $"{prefix}{nameof(Project)}",
            Comment = $"{prefix}{nameof(Project.Comment)}",
            CustomerId = customerId,
            Hidden = hidden
        };

    public static ProjectDto CreateDto(Guid? customerId = null, string prefix = "Test", bool hidden = false)
        => FakeAutoMapper.Mapper.Map<ProjectDto>(Create(customerId, prefix, hidden));
}