using FS.TimeTracking.Core.Interfaces.Repository.Services;
using FS.TimeTracking.Core.Models.Configuration;
using FS.TimeTracking.Repository.DbContexts;
using FS.TimeTracking.Repository.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;

namespace FS.TimeTracking.Repository.Startup;

internal static class DatabaseMigration
{
    public static WebApplication MigrateDatabase(this WebApplication webApplication)
    {
        var serviceFactory = webApplication.Services.GetRequiredService<IServiceScopeFactory>();
        using var serviceScope = serviceFactory.CreateScope();
        var serviceProvider = serviceScope.ServiceProvider;
        MigrateDatabase(serviceProvider);
        return webApplication;
    }

    public static IServiceProvider MigrateDatabase(this IServiceProvider serviceProvider)
    {
        var logger = serviceProvider.GetRequiredService<ILogger<TimeTrackingDbContext>>();
        using var dbContext = serviceProvider.GetRequiredService<TimeTrackingDbContext>();
        var dbTruncateService = serviceProvider.GetRequiredService<IDbTruncateService>();
        var databaseConfiguration = serviceProvider.GetRequiredService<IOptions<TimeTrackingConfiguration>>().Value.Database;

        var dbMigrationService = new DbMigrationService(logger, dbContext, dbTruncateService);
        dbMigrationService.MigrateDatabase(databaseConfiguration.TruncateOnApplicationStart);

        return serviceProvider;
    }
}