namespace FS.TimeTracking.Core.Models.Configuration;

/// <summary>
/// Database types supported by this application
/// </summary>
public enum DatabaseType
{
    /// <summary>
    /// Sqlite in memory (unit testing only)
    /// </summary>
    InMemory,

    /// <summary>
    /// SQLite
    /// </summary>
    Sqlite,

    /// <summary>
    /// Microsoft Sql Server
    /// </summary>
    SqlServer,

    /// <summary>
    /// PostgreSQL
    /// </summary>
    PostgreSql,

    /// <summary>
    /// My SQL / Maria DB
    /// </summary>
    MySql,
}