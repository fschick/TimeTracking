using AutoMapper;
using FS.TimeTracking.Application.AutoMapper;
using FS.TimeTracking.Shared.DTOs.TimeTracking;
using FS.TimeTracking.Shared.Models.TimeTracking;
using System;
using System.Diagnostics.CodeAnalysis;
using FS.TimeTracking.Shared.DTOs.MasterData;

namespace FS.TimeTracking.Shared.Tests.Services
{
    [ExcludeFromCodeCoverage]
    public static class FakeEntityFactory
    {
        public static readonly IMapper Mapper = new MapperConfiguration(cfg => cfg.AddProfile<TimeTrackingAutoMapper>()).CreateMapper();

        public static CustomerDto CreateCustomerDto(string prefix = "Test", bool hidden = false)
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

        public static ProjectDto CreateProjectDto(Guid customerId, string prefix = "Test", bool hidden = false)
            => new ProjectDto
            {
                Id = Guid.NewGuid(),
                Title = $"{prefix}Project",
                Comment = $"{prefix}Comment",
                CustomerId = customerId,
                Hidden = hidden
            };

        public static ActivityDto CreateActivityDto(Guid? projectId = null, string prefix = "Test", bool hidden = false)
            => new ActivityDto
            {
                Id = Guid.NewGuid(),
                Title = $"{prefix}Project",
                ProjectId = projectId,
                Comment = $"{prefix}Comment",
                Hidden = hidden
            };

        public static TimeSheet CreateTimeSheet(Guid projectId, Guid activityId, Guid? orderId = null, string prefix = "Test")
            => new TimeSheet
            {
                Id = Guid.NewGuid(),
                ProjectId = projectId,
                ActivityId = activityId,
                OrderId = orderId,
                StartDate = DateTimeOffset.Now.Date,
                EndDate = DateTimeOffset.Now.Date.AddHours(12),
                Billable = true,
                Comment = $"{prefix}Comment",
            };

        public static TimeSheetDto CreateTimeSheetDto(Guid projectId, Guid activityId, Guid? orderId = null, string prefix = "Test")
            => Mapper.Map<TimeSheetDto>(CreateTimeSheet(projectId, activityId, orderId, prefix));
    }
}
