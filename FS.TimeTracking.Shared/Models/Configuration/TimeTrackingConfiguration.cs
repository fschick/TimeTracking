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
    }
}
