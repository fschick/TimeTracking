using FS.TimeTracking.Abstractions.DTOs.MasterData;
using FS.TimeTracking.Core.Models.Application.MasterData;
using System;
using System.Diagnostics.CodeAnalysis;

namespace FS.TimeTracking.Application.Tests.Services;

[ExcludeFromCodeCoverage]
public static partial class FakeProject
{
    public static Project Create(Guid customerId, string prefix = "Test", bool hidden = false)
        => new()
        {
            Id = Guid.NewGuid(),
            Title = $"{prefix}{nameof(Project)}",
            Comment = $"{prefix}{nameof(Project.Comment)}",
            CustomerId = customerId,
            Hidden = hidden
        };

    public static Project Create(Customer customer, string prefix = "Test", bool hidden = false)
    {
        var result = Create(customer.Id, prefix, hidden);
        result.Customer = customer;
        return result;
    }

    public static ProjectDto CreateDto(Guid customerId, string prefix = "Test", bool hidden = false)
        => FakeAutoMapper.Mapper.Map<ProjectDto>(Create(customerId, prefix, hidden));
}