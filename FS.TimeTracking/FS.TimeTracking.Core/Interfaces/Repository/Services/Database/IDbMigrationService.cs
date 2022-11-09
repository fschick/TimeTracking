using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Core.Interfaces.Repository.Services.Database;

/// <summary>
/// Database migration service
/// </summary>
public interface IDbMigrationService
{
    /// <summary>
    /// Applies all outstanding database migrations.
    /// </summary>
    /// <param name="truncateDatabase">True, if the database should be truncated before applying migrations.</param>
    /// <param name="cancellationToken"> a token that allows processing to be cancelled.</param>
    Task MigrateDatabase(bool truncateDatabase, CancellationToken cancellationToken = default);
}