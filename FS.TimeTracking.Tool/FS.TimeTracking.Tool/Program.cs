using FS.TimeTracking.Repository.DbContexts;
using FS.TimeTracking.Tool.Interfaces.Import;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FS.TimeTracking.Tool;

public class Program
{
    public static async Task Main(string[] args)
    {
        var options = new CommandLineOptions(args);
        if (!options.Parsed)
            return;

        await using var serviceProvider = new ServiceCollection()
            .RegisterApplicationServices(options)
            .BuildServiceProvider();

        if (options.ImportKimaiV1)
        {
            MigrateDatabase(serviceProvider);
            await serviceProvider
                .GetRequiredService<IKimaiV1ImportService>()
                .Import();
        }
    }

    private static void MigrateDatabase(IServiceProvider serviceProvider)
    {
        var logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(MigrateDatabase));
        var dbContext = serviceProvider.GetRequiredService<TimeTrackingDbContext>();
        var pendingMigrations = dbContext.Database.GetPendingMigrations().ToList();
        if (pendingMigrations.Count == 0)
            return;

        logger.LogInformation("Apply migrations to database. Please be patient ...");
        foreach (var pendingMigration in pendingMigrations)
            logger.LogInformation(pendingMigration);

        dbContext.Database.Migrate();
    }
}