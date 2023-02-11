using Autofac.Extras.FakeItEasy;
using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;
using FS.TimeTracking.Application.AutoMapper;
using FS.TimeTracking.Application.Tests.Services.FakeModels;
using FS.TimeTracking.Application.Tests.Services.FakeServices;
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
    public FakeUser User { get; }

    public FakeKeycloakRepository KeycloakRepository { get; }
    public FakeAuthorizationService AuthorizationService { get; }

    public Faker(int seed, AutoFake autoFake = null)
    {
        Random = new Random(seed);
        AutoMapper = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<TimeTrackingAutoMapper>();
            cfg.AddExpressionMapping();
        }).CreateMapper();

        DateTime = new FakeDateTime();
        Guid = new FakeGuid(this);

        Filters = new FakeFilters(this);

        Customer = new FakeCustomer(this);
        Activity = new FakeActivity(this);
        Project = new FakeProject(this);
        Order = new FakeOrder(this);
        TimeSheet = new FakeTimeSheet(this);
        Holiday = new FakeHoliday(this);
        User = new FakeUser(this);

        KeycloakRepository = new FakeKeycloakRepository(this, autoFake);
        AuthorizationService = new FakeAuthorizationService(this, autoFake);
    }
}