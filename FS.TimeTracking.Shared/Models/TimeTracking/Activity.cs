using FS.TimeTracking.Shared.Interfaces.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace FS.TimeTracking.Shared.Models.TimeTracking
{
    /// <summary>
    /// Activity
    /// </summary>
    public class Activity : IEntityModel
    {
        /// <inheritdoc />
        [Required]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the short/display name.
        /// </summary>
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the identifier to the related <see cref="Customer"/>.
        /// </summary>
        public Guid? CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the identifier to the related <see cref="Project"/>.
        /// </summary>
        public Guid? ProjectId { get; set; }

        /// <summary>
        /// Gets or sets a comment.
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this item is hidden.
        /// </summary>
        [Required]
        public bool Hidden { get; set; }

        /// <inheritdoc />
        [Required]
        public DateTime Created { get; set; }

        /// <inheritdoc />
        [Required]
        public DateTime Modified { get; set; }
    }
}
