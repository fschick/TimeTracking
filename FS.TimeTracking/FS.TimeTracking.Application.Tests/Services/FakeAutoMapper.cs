using AutoMapper;
using FS.TimeTracking.Application.AutoMapper;
using System.Diagnostics.CodeAnalysis;

namespace FS.TimeTracking.Application.Tests.Services;

[ExcludeFromCodeCoverage]
public static class FakeAutoMapper
{
    public static readonly IMapper Mapper = new MapperConfiguration(cfg => cfg.AddProfile<TimeTrackingAutoMapper>()).CreateMapper();
}