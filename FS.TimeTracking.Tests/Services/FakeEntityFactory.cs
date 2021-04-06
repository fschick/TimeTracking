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
                Hidden = hidden,
            };

        public static ProjectDto CreateProject(Guid customerId, string prefix = "Test", bool hidden = false)
            => new ProjectDto
            {
                Id = Guid.NewGuid(),
                Title = $"{prefix}Project",
                Comment = $"{prefix}Comment",
                CustomerId = customerId,
                Hidden = hidden
            };

        public static ActivityDto CreateActivity(Guid? customerId = null, Guid? projectId = null, string prefix = "Test", bool hidden = false)
            => new ActivityDto
            {
                Id = Guid.NewGuid(),
                Title = $"{prefix}Project",
                CustomerId = customerId,
                ProjectId = projectId,
                Comment = $"{prefix}Comment",
                Hidden = hidden
            };

        public static TimeSheetDto CreateTimeSheet(Guid customerId, Guid activityId, string prefix = "Test")
            => new TimeSheetDto
            {
                Id = Guid.NewGuid(),
                CustomerId = customerId,
                ActivityId = activityId,
                StartDate = DateTimeOffset.Now.Date,
                EndDate = DateTimeOffset.Now.Date.AddHours(12),
                Billable = true,
                Comment = $"{prefix}Comment",
            };
    }
}
