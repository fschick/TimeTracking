using FS.TimeTracking.Abstractions.DTOs.MasterData;
using FS.TimeTracking.Core.Models.Application.MasterData;
using System.Diagnostics.CodeAnalysis;

namespace FS.TimeTracking.Application.Tests.Services.FakeModels;

[ExcludeFromCodeCoverage]
public class FakeCustomer
{
    private readonly Faker _faker;

    public FakeCustomer(Faker faker)
        => _faker = faker;

    public Customer Create(string prefix = "Test", double? hourlyRate = null, bool hidden = false)
        => new()
        {
            Id = _faker.Guid.Create(),
            Title = $"{prefix}{nameof(Customer)}",
            CompanyName = $"{prefix}{nameof(Customer.CompanyName)}",
            ContactName = $"{prefix}{nameof(Customer.ContactName)}",
            Street = $"{prefix}{nameof(Customer.Street)}",
            ZipCode = $"{prefix}{nameof(Customer.ZipCode)}",
            City = $"{prefix}{nameof(Customer.City)}",
            Country = $"{prefix}{nameof(Customer.Country)}",
            HourlyRate = hourlyRate ?? 0,
            Hidden = hidden,
        };

    public CustomerDto CreateDto(string prefix = "Test", double? hourlyRate = null, bool hidden = false)
        => _faker.AutoMapper.Map<CustomerDto>(Create(prefix, hourlyRate, hidden));
}