using FS.TimeTracking.Repository.DbContexts;
using FS.TimeTracking.Shared.Models.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace FS.TimeTracking.Repository.Startup
{
    internal static class Database
    {
        public static IApplicationBuilder MigrateDatabase(this IApplicationBuilder applicationBuilder)
        {
            var serviceFactory = applicationBuilder.ApplicationServices.GetRequiredService<IServiceScopeFactory>();
            using var scope = serviceFactory.CreateScope();
            using var dbContext = scope.ServiceProvider.GetRequiredService<TimeTrackingDbContext>();
            var configuration = scope.ServiceProvider.GetRequiredService<TimeTrackingConfiguration>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<TimeTrackingDbContext>>();

            if (configuration.Database.Type == TimeTrackingConfiguration.DatabaseType.Memory)
            {
                dbContext.Database.EnsureCreated();
            }
            else
            {
                var pendingMigrations = dbContext.Database.GetPendingMigrations().ToList();
                if (pendingMigrations.Count == 0)
                    return applicationBuilder;

                logger.LogInformation("Apply migrations to database. Please be patient ...");
                foreach (var pendingMigration in pendingMigrations)
                    logger.LogInformation(pendingMigration);
                dbContext.Database.Migrate();
                logger.LogInformation("Database migration finished.");
            }

            return applicationBuilder;
        }
    }
}
