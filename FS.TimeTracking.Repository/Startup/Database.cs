using FS.TimeTracking.Repository.DbContexts;
using FS.TimeTracking.Shared.Interfaces.Application.Services;
using FS.TimeTracking.Shared.Models.TimeTracking;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Storage;
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
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<TimeTrackingDbContext>>();
#if DEBUG
            // TODO: Remove as soon as production state has reached.
            var debugService = scope.ServiceProvider.GetRequiredService<IDebugService>();
#endif

            var pendingMigrations = dbContext.Database.GetPendingMigrations().ToList();
            if (pendingMigrations.Count == 0)
                return applicationBuilder;

            var actualStateMigrations = dbContext.Database.GetAppliedMigrations().ToList();
            var targetStateMigrations = dbContext.Database.GetMigrations().Except(pendingMigrations).ToList();
            var migrationsHasBeenMerged = !actualStateMigrations.SequenceEqual(targetStateMigrations);
            if (migrationsHasBeenMerged)
                // TODO: Remove as soon as production state has reached.
                TruncateDatabase(dbContext);

            logger.LogInformation("Apply migrations to database. Please be patient ...");
            foreach (var pendingMigration in pendingMigrations)
                logger.LogInformation(pendingMigration);
            dbContext.Database.Migrate();
            logger.LogInformation("Database migration finished.");

            return applicationBuilder;
        }

        // TODO: Remove as soon as production state has reached.
        private static void TruncateDatabase(DbContext dbContext)
        {
            var sqlGenerator = dbContext.GetInfrastructure().GetRequiredService<IMigrationsSqlGenerator>();
            var connection = dbContext.GetInfrastructure().GetRequiredService<IRelationalConnection>();

            var tableDropOperations = new[] { typeof(TimeSheet), typeof(Activity), typeof(Project), typeof(Customer) }
                .Select(type => dbContext.Model.FindEntityType(type))
                .Select(entityType => new DropTableOperation { Name = entityType.GetTableName(), Schema = entityType.GetSchema() })
                .ToList();

            var migrationTableDropOperation = new DropTableOperation { Name = HistoryRepository.DefaultTableName, Schema = tableDropOperations.First().Schema };
            tableDropOperations.Add(migrationTableDropOperation);

            var migrationCommands = sqlGenerator.Generate(tableDropOperations);
            foreach (var migrationCommand in migrationCommands)
                try { migrationCommand.ExecuteNonQuery(connection); }
                catch { /* Ignore */}
        }
    }
}
