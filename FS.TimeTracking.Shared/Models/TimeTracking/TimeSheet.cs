using FS.TimeTracking.Shared.Interfaces.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace FS.TimeTracking.Shared.Models.TimeTracking
{
    /// <summary>
    /// TimeSheet.
    /// </summary>
    public class TimeSheet : IEntityModel
    {
        /// <inheritdoc />
        [Required]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the identifier to the related <see cref="Customer"/>.
        /// </summary>
        [Required]
        public Guid CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the identifier to the related <see cref="Activity"/>.
        /// </summary>
        [Required]
        public Guid ActivityId { get; set; }

        /// <summary>
        /// Gets or sets the start time, in local time, of the current item.
        /// </summary>
        [Required]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Gets or sets the end time, in local time, of the current item.
        /// </summary>
        [Required]
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this item is billable.
        /// </summary>
        [Required]
        public bool Billable { get; set; }

        /// <summary>
        /// Gets or sets a comment.
        /// </summary>
        public string Comment { get; set; }

        /// <inheritdoc />
        [Required]
        public DateTime Created { get; set; }

        /// <inheritdoc />
        [Required]
        public DateTime Modified { get; set; }
    }
}
