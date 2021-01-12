using System;
using System.ComponentModel.DataAnnotations;

namespace FS.TimeTracking.Shared.Interfaces.Models
{
    /// <summary>
    /// Interface for entity models
    /// </summary>
    public interface IEntityModel
    {
        /// <summary>
        /// Gets the identifier.
        /// </summary>
        [Required]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the creation time, in coordinated universal time (UTC), of the current item.
        /// </summary>
        [Required]
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets the time, in coordinated universal time (UTC), when the current item was last written to.
        /// </summary>
        [Required]
        public DateTime Modified { get; set; }
    }
}
