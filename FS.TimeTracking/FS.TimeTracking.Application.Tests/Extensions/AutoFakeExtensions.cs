using Autofac.Extras.FakeItEasy;
using FS.FilterExpressionCreator.Filters;
using FS.TimeTracking.Application.FilterExpressionInterceptors;
using FS.TimeTracking.Core.Interfaces.Repository.Services;
using FS.TimeTracking.Core.Models.Configuration;
using FS.TimeTracking.Repository.DbContexts;
using FS.TimeTracking.Repository.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace FS.TimeTracking.Application.Tests.Extensions;

internal static class AutoFakeExtensions
{
    public static async Task ConfigureInMemoryDatabase(this AutoFake autoFake, TimeTrackingConfiguration configuration = null)
    {
        var connectionString = $"DataSource={Guid.NewGuid()};mode=memory;cache=shared";
        configuration ??= new TimeTrackingConfiguration();
        configuration.Database ??= new DatabaseConfiguration();
        configuration.Database.Type = DatabaseType.InMemory;
        configuration.Database.ConnectionString = connectionString;

        autoFake.Provide(Options.Create(configuration));
        autoFake.Provide(Options.Create(new EnvironmentConfiguration { IsDevelopment = true, IsProduction = false }));
        autoFake.Provide<TimeTrackingDbContext, TimeTrackingDbContext>();

        EntityFilter.DefaultInterceptor = new DateTimeOffsetInterceptor();
        var dbContext = autoFake.Resolve<TimeTrackingDbContext>();
        await dbContext.Database.MigrateAsync();
    }

    public static async Task ConfigureSqlServerDebugDatabase(this AutoFake autoFake, TimeTrackingConfiguration configuration = null)
    {
        const string connectionString = "Data Source=.;Initial Catalog=TimeTracking.Debug;Trusted_Connection=True;Persist Security Info=True";
        configuration ??= new TimeTrackingConfiguration();
        configuration.Database ??= new DatabaseConfiguration();
        configuration.Database.Type = DatabaseType.SqlServer;
        configuration.Database.ConnectionString = connectionString;

        autoFake.Provide(Options.Create(configuration));
        autoFake.Provide(Options.Create(new EnvironmentConfiguration { IsDevelopment = true, IsProduction = false }));
        autoFake.Provide<TimeTrackingDbContext, TimeTrackingDbContext>();
        autoFake.Provide<ITruncateDbService, TruncateDbService>();

        EntityFilter.DefaultInterceptor = new DateTimeOffsetInterceptor();
        var truncateDbService = autoFake.Resolve<ITruncateDbService>();
        truncateDbService.TruncateDatabase();
        var dbContext = autoFake.Resolve<TimeTrackingDbContext>();
        await dbContext.Database.MigrateAsync();
    }
}