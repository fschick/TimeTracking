namespace FS.TimeTracking.Shared.Models.Configuration
{
    /// <summary>
    /// Global environment configurations
    /// </summary>
    public class EnvironmentConfiguration
    {
        /// <summary>
        /// Indicates whether development environment is active.
        /// </summary>
        public bool IsDevelopment { get; set; }

        /// <summary>
        /// Indicates whether production environment is active.
        /// </summary>
        public bool IsProduction { get; set; }
    }
}
