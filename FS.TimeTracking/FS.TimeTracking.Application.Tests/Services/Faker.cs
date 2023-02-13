using Autofac.Core;
using Autofac.Extras.FakeItEasy;
using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;
using FS.TimeTracking.Application.AutoMapper;
using FS.TimeTracking.Application.Services.Shared;
using FS.TimeTracking.Application.Tests.Services.FakeModels;
using FS.TimeTracking.Application.Tests.Services.FakeServices;
using FS.TimeTracking.Core.Interfaces.Application.Services.Shared;
using System;

namespace FS.TimeTracking.Application.Tests.Services;

public sealed class Faker : IServiceProvider, IDisposable
{
    public AutoFake AutoFake { get; }
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

    public Faker(int seed = 2000)
    {
        Random = new Random(seed);
        AutoMapper = CreateAutoMapper();
        AutoFake = CreateAutoFake(AutoMapper);

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

        KeycloakRepository = new FakeKeycloakRepository(this);
        AuthorizationService = new FakeAuthorizationService(this);
    }

    public void Dispose()
        => AutoFake?.Dispose();

    private static IMapper CreateAutoMapper()
        => new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<TimeTrackingAutoMapper>();
            cfg.AddExpressionMapping();
        }).CreateMapper();

    private static AutoFake CreateAutoFake(IMapper autoMapper)
    {
        var autoFake = new AutoFake();
        autoFake.Provide(autoMapper);
        autoFake.Provide<IFilterFactory, FilterFactory>();
        return autoFake;
    }

    public object GetService(Type serviceType)
    {
        var autoFakeResolve = AutoFake.GetType().GetMethod(nameof(AutoFake.Resolve));
        var genericResolve = autoFakeResolve!.MakeGenericMethod(serviceType);
        var requestedService = genericResolve.Invoke(AutoFake, new object[] { Array.Empty<Parameter>() });
        return requestedService;
    }
}