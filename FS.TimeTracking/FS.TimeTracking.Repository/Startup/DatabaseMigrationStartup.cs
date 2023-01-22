using FS.TimeTracking.Core.Interfaces.Repository.Services.Database;
using FS.TimeTracking.Core.Models.Configuration;
using FS.TimeTracking.Repository.DbContexts;
using FS.TimeTracking.Repository.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Repository.Startup;

internal static class DatabaseMigrationStartup
{
    public static async Task MigrateDatabase(this WebApplication webApplication, CancellationToken cancellationToken = default)
    {
        var serviceFactory = webApplication.Services.GetRequiredService<IServiceScopeFactory>();
        using var serviceScope = serviceFactory.CreateScope();
        var serviceProvider = serviceScope.ServiceProvider;
        await MigrateDatabase(serviceProvider, cancellationToken);
    }

    private static async Task MigrateDatabase(this IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        var logger = serviceProvider.GetRequiredService<ILogger<TimeTrackingDbContext>>();
        await using var dbContext = serviceProvider.GetRequiredService<TimeTrackingDbContext>();
        var dbTruncateService = serviceProvider.GetRequiredService<IDbTruncateService>();
        var databaseConfiguration = serviceProvider.GetRequiredService<IOptions<TimeTrackingConfiguration>>().Value.Database;

        var dbMigrationService = new DbMigrationService(logger, dbContext, dbTruncateService);
        await dbMigrationService.MigrateDatabase(databaseConfiguration.TruncateOnApplicationStart, cancellationToken);
    }
}