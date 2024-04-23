using Autofac.Core;
using AutoMapper;
using FS.Keycloak.RestApiClient.Model;
using FS.TimeTracking.Abstractions.DTOs.Administration;
using FS.TimeTracking.Application.FilterExpressionInterceptors;
using FS.TimeTracking.Application.Services.Administration;
using FS.TimeTracking.Application.Tests.Services;
using FS.TimeTracking.Core.Interfaces.Application.Services.Administration;
using FS.TimeTracking.Core.Interfaces.Repository.Services.Administration;
using FS.TimeTracking.Core.Interfaces.Repository.Services.Database;
using FS.TimeTracking.Core.Models.Configuration;
using FS.TimeTracking.Repository.DbContexts;
using FS.TimeTracking.Repository.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Plainquire.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FS.TimeTracking.Application.Tests.Extensions;

internal static class FakerExtensions
{
    public static Faker Configure(this Faker faker, Action<TimeTrackingConfiguration> configure)
    {
        var configurationOptions = faker.GetRequiredService<IOptions<TimeTrackingConfiguration>>();
        var configuration = configurationOptions.Value;
        configure.Invoke(configuration);
        faker.Provide(Options.Create(configuration));
        return faker;
    }

    public static Faker ConfigureInMemoryDatabase(this Faker faker)
    {
        var databaseType = DatabaseType.InMemory;
        var connectionString = $"DataSource={Guid.NewGuid()};mode=memory;cache=shared";
        return faker.ConfigureDatabase(databaseType, connectionString);
    }

    public static Faker ConfigureSqlServerDebugDatabase(this Faker faker)
    {
        var databaseType = DatabaseType.SqlServer;
        var connectionString = "Data Source=srv-sql-1;Initial Catalog=FS.TimeTracking.Debug;Trusted_Connection=True;Persist Security Info=True";
        return faker.ConfigureDatabase(databaseType, connectionString);
    }

    public static Faker ConfigureAuthorization(this Faker faker, bool enabled, UserDto currentUser = null)
    {
        faker.Configure(cfg => cfg.Features.Authorization = enabled);
        faker.Provide(faker.KeycloakRepository.Create());
        faker.Provide(faker.AuthorizationService.Create(currentUser));
        faker.Provide<IUserService, UserService>();
        return faker;
    }

    public static async Task<Faker> ProvideUsers(this Faker faker, params UserDto[] users)
    {
        var mapper = faker.GetRequiredService<IMapper>();
        var keycloakConfiguration = faker.GetRequiredService<KeycloakConfiguration>();
        var keycloakRepository = faker.GetRequiredService<IKeycloakRepository>();

        var keycloakRealm = keycloakConfiguration.Realm;
        var keycloakClientId = keycloakConfiguration.ClientId;
        var allClientRoles = await keycloakRepository.GetClientRoles(keycloakRealm, keycloakClientId);

        foreach (var userDto in users)
        {
            var user = mapper.Map<UserRepresentation>(userDto);
            await keycloakRepository.CreateUser(keycloakRealm, user);
            var userRoleNames = mapper.Map<List<string>>(userDto.Permissions);
            var userRoles = allClientRoles.Where(x => userRoleNames.Contains(x.Name)).ToList();
            await keycloakRepository.AddClientRolesToUser(keycloakRealm, userDto.Id, keycloakClientId, userRoles);
        }

        return faker;
    }

    public static TService Provide<TService, TImplementation>(this Faker faker, params Parameter[] parameters)
        => faker.AutoFake.Provide<TService, TImplementation>(parameters);

    public static TService Provide<TService>(this Faker faker, TService instance) where TService : class
        => faker.AutoFake.Provide(instance);

    private static Faker ConfigureDatabase(this Faker faker, DatabaseType databaseType, string connectionString)
    {
        faker.Configure(configuration =>
        {
            configuration.Database.Type = databaseType;
            configuration.Database.ConnectionString = connectionString;
        });

        faker.Provide<IDbRepository, DbRepository<TimeTrackingDbContext>>();
        faker.Provide<TimeTrackingDbContext, TimeTrackingDbContext>();

        IFilterInterceptor.Default = new DateTimeOffsetInterceptor();
        var dbContext = faker.GetRequiredService<TimeTrackingDbContext>();
        dbContext.Database.Migrate();

        return faker;
    }
}