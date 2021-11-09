﻿using FS.TimeTracking.Shared.Extensions;
using FS.TimeTracking.Shared.Interfaces.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FS.TimeTracking.Shared.Models.TimeTracking
{
    /// <summary>
    /// Time sheet position.
    /// </summary>
    public class TimeSheet : IEntityModel
    {
        /// <inheritdoc />
        [Required]
        public Guid Id { get; set; }

        /// <summary>
        /// The start date in local time.
        /// </summary>
        public DateTime StartDateLocal { get; set; }

        /// <summary>
        /// The start date's timezone offset in hours.
        /// </summary>
        public int StartDateOffset { get; set; }

        /// <summary>
        /// The start date.
        /// </summary>
        [Required]
        [NotMapped]
        public DateTimeOffset StartDate
        {
            get => StartDateLocal.ToOffset(TimeSpan.FromMinutes(StartDateOffset));
            set { StartDateLocal = value.DateTime; StartDateOffset = (int)value.Offset.TotalMinutes; }
        }

        /// <summary>
        /// The end date in UTC.
        /// </summary>
        public DateTime? EndDateLocal { get; set; }

        /// <summary>
        /// The end date's timezone offset in hours.
        /// </summary>
        public int? EndDateOffset { get; set; }

        /// <summary>
        /// The end date.
        /// </summary>
        [NotMapped]
        public DateTimeOffset? EndDate
        {
            get => EndDateLocal?.ToOffset(TimeSpan.FromMinutes(EndDateOffset!.Value));
            set { EndDateLocal = value?.DateTime; EndDateOffset = (int?)value?.Offset.TotalMinutes; }
        }

        /// <summary>
        /// Comment for this item.
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// The related issue/ticket/... .
        /// </summary>
        public string Issue { get; set; }

        /// <summary>
        /// The identifier to the related <see cref="Project"/>.
        /// </summary>
        [Required]
        public Guid ProjectId { get; set; }

        /// <inheritdoc cref="TimeTracking.Project"/>
        public Project Project { get; set; }

        /// <summary>
        /// The identifier to the related <see cref="Activity"/>.
        /// </summary>
        [Required]
        public Guid ActivityId { get; set; }

        /// <inheritdoc cref="TimeTracking.Activity"/>
        public Activity Activity { get; set; }

        /// <summary>
        /// The identifier to the related <see cref="Order"/>.
        /// </summary>
        public Guid? OrderId { get; set; }

        /// <inheritdoc cref="TimeTracking.Order"/>
        public Order Order { get; set; }

        /// <summary>
        /// Indicates whether this item is billable.
        /// </summary>
        [Required]
        public bool Billable { get; set; }

        /// <inheritdoc />
        [Required]
        public DateTime Created { get; set; }

        /// <inheritdoc />
        [Required]
        public DateTime Modified { get; set; }
    }
}
