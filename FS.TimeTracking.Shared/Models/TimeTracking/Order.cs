using FS.TimeTracking.Shared.Extensions;
using FS.TimeTracking.Shared.Interfaces.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FS.TimeTracking.Shared.Models.TimeTracking
{
    /// <summary>
    /// Project
    /// </summary>
    public class Order : IEntityModel
    {
        /// <inheritdoc />
        [Required]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets the short/display name.
        /// </summary>
        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets a description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets the order number.
        /// </summary>
        [StringLength(100)]
        public string Number { get; set; }

        /// <summary>
        /// Gets or sets the identifier to the related <see cref="Customer"/>.
        /// </summary>
        [Required]
        public Guid CustomerId { get; set; }

        /// <inheritdoc cref="TimeTracking.Customer"/>
        public Customer Customer { get; set; }

        /// <summary>
        /// Gets the start date in UTC.
        /// </summary>
        public DateTime StartDateUtc { get; set; }

        /// <summary>
        /// Gets the start date's timezone offset in hours.
        /// </summary>
        public double StartDateOffset { get; set; }

        /// <summary>
        /// Gets the start date.
        /// </summary>
        [Required]
        [NotMapped]
        public DateTimeOffset StartDate
        {
            get => StartDateUtc.ToOffset(TimeSpan.FromHours(StartDateOffset));
            set { StartDateUtc = value.UtcDateTime; StartDateOffset = value.Offset.TotalHours; }
        }

        /// <summary>
        /// Gets the due date in UTC.
        /// </summary>
        public DateTime DueDateUtc { get; set; }

        /// <summary>
        /// Gets the due date's timezone offset in hours.
        /// </summary>
        public double DueDateOffset { get; set; }

        /// <summary>
        /// Gets the due date.
        /// </summary>
        [Required]
        [NotMapped]
        public DateTimeOffset DueDate
        {
            get => DueDateUtc.ToOffset(TimeSpan.FromHours(DueDateOffset));
            set { DueDateUtc = value.UtcDateTime; DueDateOffset = value.Offset.TotalHours; }
        }

        /// <summary>
        /// Gets the hourly rate.
        /// </summary>
        /// [Required]
        [Range(0, double.PositiveInfinity)]
        public double HourlyRate { get; set; }

        /// <summary>
        /// Gets the budget.
        /// </summary>
        [Required]
        [Range(0, double.PositiveInfinity)]
        public double Budget { get; set; }

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
