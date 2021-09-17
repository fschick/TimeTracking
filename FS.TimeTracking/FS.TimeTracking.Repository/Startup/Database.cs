using FS.TimeTracking.Repository.DbContexts;
using FS.TimeTracking.Shared.Interfaces.Services;
using FS.TimeTracking.Shared.Models.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Linq;

namespace FS.TimeTracking.Repository.Startup
{
    internal static class Database
    {
        public static IApplicationBuilder MigrateDatabase(this IApplicationBuilder applicationBuilder)
        {
            var serviceFactory = applicationBuilder.ApplicationServices.GetRequiredService<IServiceScopeFactory>();
            using var serviceScope = serviceFactory.CreateScope();
            var databaseConfiguration = serviceScope.ServiceProvider.GetRequiredService<IOptions<TimeTrackingConfiguration>>().Value.Database;
            using var dbContext = serviceScope.ServiceProvider.GetRequiredService<TimeTrackingDbContext>();
            var logger = serviceScope.ServiceProvider.GetRequiredService<ILogger<TimeTrackingDbContext>>();
            var truncateDbService = serviceScope.ServiceProvider.GetRequiredService<ITruncateDbService>();

            if (databaseConfiguration.TruncateOnApplicationStart)
                truncateDbService.TruncateDatabase();

            var pendingMigrations = dbContext.Database.GetPendingMigrations().ToList();
            if (pendingMigrations.Count == 0)
                return applicationBuilder;

            logger.LogInformation("Apply migrations to database. Please be patient ...");
            foreach (var pendingMigration in pendingMigrations)
                logger.LogInformation(pendingMigration);
#if DEBUG
            try
            {
                dbContext.Database.Migrate();
            }
            catch
            {
                truncateDbService.TruncateDatabase();
                dbContext.Database.Migrate();
            }
#else
            dbContext.Database.Migrate();
#endif
            logger.LogInformation("Database migration finished.");

            return applicationBuilder;
        }
    }
}
