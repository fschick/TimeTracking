using FS.TimeTracking.Shared.DTOs.TimeTracking;
using System;

namespace FS.TimeTracking.Tests.Services
{
    public static class FakeEntityFactory
    {
        public static CustomerDto CreateCustomer(string prefix = "Test", bool hidden = false)
            => new CustomerDto
            {
                Id = Guid.NewGuid(),
                Title = $"{prefix}Customer",
                CompanyName = $"{prefix}Company",
                ContactName = $"{prefix}Contact",
                Street = $"{prefix}Street",
                ZipCode = "1234",
                City = $"{prefix}City",
                Country = $"{prefix}Country",
                Hidden = false,
            };
        public static ProjectDto CreateProject(Guid customerId, string prefix = "Test", bool hidden = false)
            => new ProjectDto
            {
                Id = Guid.NewGuid(),
                Title = $"{prefix}Name",
                Comment = $"{prefix}Comment",
                CustomerId = customerId,
                Hidden = hidden
            };
    }
}
