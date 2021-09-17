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
        /// Gets or sets the identifier to the related <see cref="Project"/>.
        /// </summary>
        [Required]
        public Guid ProjectId { get; set; }

        /// <summary>
        /// Gets or sets the identifier to the related <see cref="Activity"/>.
        /// </summary>
        [Required]
        public Guid ActivityId { get; set; }

        /// <summary>
        /// Gets or sets the identifier to the related <see cref="Order"/>.
        /// </summary>
        public Guid? OrderId { get; set; }

        /// <summary>
        /// Gets or sets the related issue/ticket/... .
        /// </summary>
        public string Issue { get; set; }

        /// <summary>
        /// Gets the start date in local time.
        /// </summary>
        public DateTime StartDateLocal { get; set; }

        /// <summary>
        /// Gets the start date's timezone offset in hours.
        /// </summary>
        public int StartDateOffset { get; set; }

        /// <summary>
        /// Gets the start date.
        /// </summary>
        [Required]
        [NotMapped]
        public DateTimeOffset StartDate
        {
            get => StartDateLocal.ToOffset(TimeSpan.FromMinutes(StartDateOffset));
            set { StartDateLocal = value.DateTime; StartDateOffset = (int)value.Offset.TotalMinutes; }
        }

        /// <summary>
        /// Gets the end date in UTC.
        /// </summary>
        public DateTime? EndDateLocal { get; set; }

        /// <summary>
        /// Gets the end date's timezone offset in hours.
        /// </summary>
        public int? EndDateOffset { get; set; }

        /// <summary>
        /// Gets the end date.
        /// </summary>
        [NotMapped]
        public DateTimeOffset? EndDate
        {
            get => EndDateLocal?.ToOffset(TimeSpan.FromMinutes(EndDateOffset!.Value));
            set { EndDateLocal = value?.DateTime; EndDateOffset = (int?)value?.Offset.TotalMinutes; }
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
