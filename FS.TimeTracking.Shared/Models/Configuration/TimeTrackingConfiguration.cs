namespace FS.TimeTracking.Shared.Models.Configuration
{
    /// <summary>
    /// Global application configuration
    /// </summary>
    public class TimeTrackingConfiguration
    {
        /// <summary>
        /// The configuration section bind to.
        /// </summary>
        public const string CONFIGURATION_SECTION = "TimeTracking";

        /// <summary>
        /// Gets or sets the database configuration.
        /// </summary>
        public DatabaseConfiguration Database { get; set; } = new();

        /// <summary>
        /// Database specific configuration
        /// </summary>
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
        }

        /// <summary>
        /// Database types supported by this application
        /// </summary>
        public enum DatabaseType
        {
            /// <summary>
            /// In-Memory database (testing only)
            /// </summary>
            Memory,

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
    }
}
