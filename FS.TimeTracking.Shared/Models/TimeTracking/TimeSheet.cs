using FS.TimeTracking.Shared.Extensions;
using FS.TimeTracking.Shared.Interfaces.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        /// Gets the end date in UTC.
        /// </summary>
        public DateTime? EndDateUtc { get; set; }

        /// <summary>
        /// Gets the end date's timezone offset in hours.
        /// </summary>
        public double? EndDateOffset { get; set; }

        /// <summary>
        /// Gets the end date.
        /// </summary>
        [NotMapped]
        public DateTimeOffset? EndDate
        {
            get => EndDateUtc.ToOffset(TimeSpan.FromHours(EndDateOffset!.Value));
            set { EndDateUtc = value?.UtcDateTime; EndDateOffset = value?.Offset.TotalHours; }
        }

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
