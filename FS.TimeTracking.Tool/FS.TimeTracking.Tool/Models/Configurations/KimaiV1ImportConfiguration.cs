using FS.TimeTracking.Core.Models.Configuration;

namespace FS.TimeTracking.Tool.Models.Configurations;

public class KimaiV1ImportConfiguration
{
    /// <summary>
    /// The connection string of the database.
    /// </summary>
    public string SourceConnectionString { get; set; }

    /// <summary>
    /// The type of database.
    /// </summary>
    public DatabaseType SourceDatabaseType { get; set; }

    /// <summary>
    /// The prefix for table names in kimai database.
    /// </summary>
    public string TablePrefix { get; set; }

    /// <summary>
    /// Truncate database before import.
    /// </summary>
    public bool TruncateBeforeImport { get; set; }
}