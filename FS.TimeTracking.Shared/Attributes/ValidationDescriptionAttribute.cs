using System;

namespace FS.TimeTracking.Shared.Attributes
{
    /// <summary>
    /// If <see cref="Enabled" /> is set, property validation attributes are exported from validation description services.
    /// Implements the <see cref="System.Attribute" />
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class ValidationDescriptionAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets a value indicating whether property validation attributes are exported from validation description services.
        /// </summary>
        public bool Enabled { get; set; } = true;
    }
}
