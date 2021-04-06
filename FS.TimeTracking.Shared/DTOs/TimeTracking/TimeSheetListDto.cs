using FS.TimeTracking.Shared.Attributes;
using FS.TimeTracking.Shared.Models.TimeTracking;
using System;
using System.ComponentModel.DataAnnotations;

namespace FS.TimeTracking.Shared.DTOs.TimeTracking
{
    /// <inheritdoc cref="TimeSheet"/>
    [ValidationDescription]
    public class TimeSheetListDto
    {
        /// <inheritdoc cref="TimeSheet.Id"/>
        [Required]
        public Guid Id { get; set; }

        /// <inheritdoc cref="TimeSheet.StartDate"/>
        [Required]
        public DateTimeOffset StartDate { get; set; }

        /// <inheritdoc cref="TimeSheet.EndDate"/>
        public DateTimeOffset? EndDate { get; set; }

        /// <inheritdoc cref="TimeSheet.Billable"/>
        [Required]
        public bool Billable { get; set; }

        /// <inheritdoc cref="TimeSheet.Comment"/>
        public string Comment { get; set; }
    }
}
