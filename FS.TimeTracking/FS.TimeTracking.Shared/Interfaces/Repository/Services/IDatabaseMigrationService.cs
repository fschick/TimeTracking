namespace FS.TimeTracking.Shared.Interfaces.Repository.Services;

/// <summary>
/// Database migration service
/// </summary>
public interface IDatabaseMigrationService
{
    /// <summary>
    /// Applies all outstanding database migrations.
    /// </summary>
    /// <param name="truncateDatabase">True, if the database should be truncated before applying migrations.</param>
    void MigrateDatabase(bool truncateDatabase);
}