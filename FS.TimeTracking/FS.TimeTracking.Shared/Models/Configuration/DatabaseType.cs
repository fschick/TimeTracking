namespace FS.TimeTracking.Shared.Models.Configuration;

/// <summary>
/// Database types supported by this application
/// </summary>
public enum DatabaseType
{
    /// <summary>
    /// SQLite
    /// </summary>
    SqLite,

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