namespace FS.TimeTracking.Core.Interfaces.Repository.Services.Database;

/// <summary>
/// Services to truncate whole database without removing the database itself
/// </summary>
public interface IDbTruncateService
{
    /// <summary>
    /// Truncates the database without removing itself.
    /// </summary>
    void TruncateDatabase();
}