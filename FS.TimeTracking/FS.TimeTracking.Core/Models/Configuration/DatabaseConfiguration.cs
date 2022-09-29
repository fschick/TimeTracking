using Newtonsoft.Json;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace FS.TimeTracking.Core.Models.Configuration;

/// <summary>
/// Database specific configuration
/// </summary>
[ExcludeFromCodeCoverage]
[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
public class DatabaseConfiguration
{
    /// <summary>
    /// The type of the database.
    /// </summary>
    public DatabaseType Type { get; set; }

    /// <summary>
    /// The connection string to use.
    /// </summary>
    public string ConnectionString { get; set; }

    /// <summary>
    /// Truncates the database (remove all tables) before start. Should be used for testing purposes only.
    /// </summary>
    public bool TruncateOnApplicationStart { get; set; }

    [JsonIgnore]
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => $"{Type}, {ConnectionString}";
}