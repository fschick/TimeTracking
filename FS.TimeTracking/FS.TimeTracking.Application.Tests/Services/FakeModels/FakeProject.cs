using FS.TimeTracking.Abstractions.DTOs.MasterData;
using FS.TimeTracking.Core.Models.Application.MasterData;
using System;
using System.Diagnostics.CodeAnalysis;

namespace FS.TimeTracking.Application.Tests.Services.FakeModels;

[ExcludeFromCodeCoverage]
public class FakeProject
{
    private readonly Faker _faker;

    public FakeProject(Faker faker)
        => _faker = faker;

    public Project Create(Guid? customerId = null, string prefix = "Test", bool hidden = false)
        => new()
        {
            Id = _faker.Guid.Create(),
            Title = $"{prefix}{nameof(Project)}",
            Comment = $"{prefix}{nameof(Project.Comment)}",
            CustomerId = customerId,
            Hidden = hidden
        };

    public ProjectDto CreateDto(Guid? customerId = null, string prefix = "Test", bool hidden = false)
        => _faker.AutoMapper.Map<ProjectDto>(Create(customerId, prefix, hidden));
}