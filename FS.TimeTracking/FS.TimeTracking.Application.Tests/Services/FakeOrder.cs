using FS.TimeTracking.Abstractions.DTOs.MasterData;
using FS.TimeTracking.Core.Extensions;
using FS.TimeTracking.Core.Models.Application.MasterData;
using System;
using System.Diagnostics.CodeAnalysis;

namespace FS.TimeTracking.Application.Tests.Services;

[ExcludeFromCodeCoverage]
public static partial class FakeOrder
{
    public static Order Create(Guid customerId, DateTimeOffset? startDate = null, DateTimeOffset? dueDate = null, string number = null, double? hourlyRate = null, double? budget = null, string prefix = "Test", bool hidden = false)
        => new()
        {
            Id = Guid.NewGuid(),
            Title = $"{prefix}{nameof(Order)}",
            Number = number ?? Guid.NewGuid().GetHashCode().ToString(),
            CustomerId = customerId,
            StartDate = startDate ?? DateTimeOffset.Now.StartOfYear(),
            DueDate = dueDate ?? DateTimeOffset.Now.EndOfYear(),
            HourlyRate = hourlyRate ?? 100,
            Budget = budget ?? 500,
            Comment = $"{prefix}{nameof(Order.Comment)}",
            Hidden = hidden
        };

    public static OrderDto CreateDto(Guid customerId, DateTimeOffset? startDate = null, DateTimeOffset? dueDate = null, string number = null, double? hourlyRate = null, double? budget = null, string prefix = "Test", bool hidden = false)
        => FakeAutoMapper.Mapper.Map<OrderDto>(Create(customerId, startDate, dueDate, number, hourlyRate, budget, prefix, hidden));
}