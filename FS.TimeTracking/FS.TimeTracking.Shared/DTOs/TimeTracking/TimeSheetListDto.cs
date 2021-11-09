using FS.TimeTracking.Shared.Models.TimeTracking;
using System;
using System.ComponentModel.DataAnnotations;

namespace FS.TimeTracking.Shared.DTOs.TimeTracking
{
    /// <inheritdoc cref="TimeSheet"/>
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

        /// <summary>
        /// Total working time for this sheet.
        /// </summary>
        public TimeSpan? Duration { get; set; }

        /// <inheritdoc cref="TimeSheet.Comment"/>
        public string Comment { get; set; }

        /// <inheritdoc cref="TimeSheet.Issue"/>
        public string Issue { get; set; }

        /// <inheritdoc cref="Customer.Title"/>
        public string CustomerTitle { get; set; }

        /// <inheritdoc cref="Project.Title"/>
        public string ProjectTitle { get; set; }

        /// <inheritdoc cref="Activity.Title"/>
        public string ActivityTitle { get; set; }

        /// <inheritdoc cref="TimeSheet.Billable"/>
        [Required]
        public bool Billable { get; set; }
    }
}
