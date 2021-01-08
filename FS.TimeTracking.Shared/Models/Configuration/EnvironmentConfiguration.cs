namespace FS.TimeTracking.Shared.Models.Configuration
{
    /// <summary>
    /// Global environment configurations
    /// </summary>
    public class EnvironmentConfiguration
    {
        /// <summary>
        /// Gets or sets a value indicating whether development environment is active.
        /// </summary>
        public bool IsDevelopment { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether production environment is active.
        /// </summary>
        public bool IsProduction { get; set; }
    }
}
