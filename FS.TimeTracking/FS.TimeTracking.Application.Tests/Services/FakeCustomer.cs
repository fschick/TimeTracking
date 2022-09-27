using FS.TimeTracking.Abstractions.DTOs.MasterData;
using FS.TimeTracking.Core.Models.Application.MasterData;
using System;
using System.Diagnostics.CodeAnalysis;

namespace FS.TimeTracking.Application.Tests.Services;

[ExcludeFromCodeCoverage]
public static class FakeCustomer
{
    public static Customer Create(string prefix = "Test", bool hidden = false)
        => new()
        {
            Id = Guid.NewGuid(),
            Title = $"{prefix}{nameof(Customer)}",
            CompanyName = $"{prefix}{nameof(Customer.CompanyName)}",
            ContactName = $"{prefix}{nameof(Customer.ContactName)}",
            Street = $"{prefix}{nameof(Customer.Street)}",
            ZipCode = $"{prefix}{nameof(Customer.ZipCode)}",
            City = $"{prefix}{nameof(Customer.City)}",
            Country = $"{prefix}{nameof(Customer.Country)}",
            Hidden = hidden,
        };

    public static CustomerDto CreateDto(string prefix = "Test", bool hidden = false)
        => FakeAutoMapper.Mapper.Map<CustomerDto>(Create(prefix, hidden));
}