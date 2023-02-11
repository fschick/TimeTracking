using FS.TimeTracking.Abstractions.DTOs.MasterData;
using FS.TimeTracking.Core.Extensions;
using FS.TimeTracking.Core.Models.Application.MasterData;
using System;
using System.Diagnostics.CodeAnalysis;

namespace FS.TimeTracking.Application.Tests.Services.FakeModels;

[ExcludeFromCodeCoverage]
public class FakeOrder
{
    private readonly Faker _faker;

    public FakeOrder(Faker faker)
        => _faker = faker;

    public Order Create(Guid customerId, DateTimeOffset? startDate = null, DateTimeOffset? dueDate = null, string number = null, double? hourlyRate = null, double? budget = null, string prefix = "Test", bool hidden = false)
        => new()
        {
            Id = _faker.Guid.Create(),
            Title = $"{prefix}{nameof(Order)}",
            Number = number ?? _faker.Guid.Create().GetHashCode().ToString(),
            CustomerId = customerId,
            StartDate = startDate ?? DateTimeOffset.Now.StartOfYear(),
            DueDate = dueDate ?? DateTimeOffset.Now.EndOfYear(),
            HourlyRate = hourlyRate ?? 100,
            Budget = budget ?? 500,
            Comment = $"{prefix}{nameof(Order.Comment)}",
            Hidden = hidden
        };

    public OrderDto CreateDto(Guid customerId, DateTimeOffset? startDate = null, DateTimeOffset? dueDate = null, string number = null, double? hourlyRate = null, double? budget = null, string prefix = "Test", bool hidden = false)
        => _faker.AutoMapper.Map<OrderDto>(Create(customerId, startDate, dueDate, number, hourlyRate, budget, prefix, hidden));
}