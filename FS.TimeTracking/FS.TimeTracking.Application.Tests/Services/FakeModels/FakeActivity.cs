using FS.TimeTracking.Abstractions.DTOs.MasterData;
using FS.TimeTracking.Core.Models.Application.MasterData;
using System;
using System.Diagnostics.CodeAnalysis;

namespace FS.TimeTracking.Application.Tests.Services.FakeModels;

[ExcludeFromCodeCoverage]
public class FakeActivity
{
    private readonly Faker _faker;

    public FakeActivity(Faker faker)
        => _faker = faker;

    public Activity Create(Guid? customerId = null, Guid? projectId = null, string prefix = "Test", bool hidden = false)
        => new()
        {
            Id = _faker.Guid.Create(),
            Title = $"{prefix}{nameof(Activity)}",
            CustomerId = customerId,
            ProjectId = projectId,
            Comment = $"{prefix}{nameof(Activity.Comment)}",
            Hidden = hidden
        };

    public ActivityDto CreateDto(Guid? customerId = null, Guid? projectId = null, string prefix = "Test", bool hidden = false)
        => _faker.AutoMapper.Map<ActivityDto>(Create(customerId, projectId, prefix, hidden));
}