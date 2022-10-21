using AutoMapper;
using FS.TimeTracking.Application.AutoMapper;
using System;

namespace FS.TimeTracking.Application.Tests.Services;

public class Faker
{
    public Random Random { get; }
    public IMapper AutoMapper { get; }
    public FakeDateTime DateTime { get; }
    public FakeGuid Guid { get; }
    public FakeFilters Filters { get; }
    public FakeCustomer Customer { get; }
    public FakeActivity Activity { get; }
    public FakeProject Project { get; }
    public FakeOrder Order { get; }
    public FakeTimeSheet TimeSheet { get; }
    public FakeHoliday Holiday { get; }

    public Faker(int seed)
    {
        Random = new Random(seed);
        AutoMapper = new MapperConfiguration(cfg => cfg.AddProfile<TimeTrackingAutoMapper>()).CreateMapper();
        DateTime = new FakeDateTime();
        Guid = new FakeGuid(this);
        Filters = new FakeFilters(this);
        Customer = new FakeCustomer(this);
        Activity = new FakeActivity(this);
        Project = new FakeProject(this);
        Order = new FakeOrder(this);
        TimeSheet = new FakeTimeSheet(this);
        Holiday = new FakeHoliday(this);
    }
}