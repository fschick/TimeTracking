﻿using Newtonsoft.Json;
using System.Diagnostics;

namespace FS.TimeTracking.Shared.Models.Configuration
{
    /// <summary>
    /// Database specific configuration
    /// </summary>
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public class DatabaseConfiguration
    {
        /// <summary>
        /// Gets or sets the type of the database.
        /// </summary>
        public DatabaseType Type { get; set; }

        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        public string ConnectionString { get; set; }

        [JsonIgnore]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay => $"{Type}, {ConnectionString}";
    }
}
