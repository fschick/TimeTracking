using FS.TimeTracking.Shared.Models.TimeTracking;
using System;
using System.ComponentModel.DataAnnotations;

namespace FS.TimeTracking.Shared.DTOs.TimeTracking
{
    /// <inheritdoc cref="TimeSheet"/>
    public class TimeSheetDto
    {
        /// <inheritdoc cref="TimeSheet.Id"/>
        [Required]
        public Guid Id { get; set; }

        /// <inheritdoc cref="TimeSheet.CustomerId"/>
        [Required]
        public Guid CustomerId { get; set; }

        /// <inheritdoc cref="TimeSheet.ActivityId"/>
        [Required]
        public Guid ActivityId { get; set; }

        /// <inheritdoc cref="TimeSheet.StartTime"/>
        [Required]
        public DateTime StartTime { get; set; }

        /// <inheritdoc cref="TimeSheet.EndTime"/>
        [Required]
        public DateTime EndTime { get; set; }

        /// <inheritdoc cref="TimeSheet.Billable"/>
        [Required]
        public bool Billable { get; set; }

        /// <inheritdoc cref="TimeSheet.Comment"/>
        public string Comment { get; set; }
    }
}
