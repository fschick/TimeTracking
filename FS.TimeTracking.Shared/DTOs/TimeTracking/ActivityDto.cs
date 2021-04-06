using FS.TimeTracking.Shared.Attributes;
using FS.TimeTracking.Shared.Models.TimeTracking;
using System;
using System.ComponentModel.DataAnnotations;

namespace FS.TimeTracking.Shared.DTOs.TimeTracking
{
    /// <inheritdoc cref="Activity"/>
    [ValidationDescription]
    public class ActivityDto
    {
        /// <inheritdoc cref="Activity.Id"/>
        [Required]
        public Guid Id { get; set; }

        /// <inheritdoc cref="Activity.Title"/>
        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        /// <inheritdoc cref="Activity.ProjectId"/>
        public Guid? ProjectId { get; set; }

        /// <inheritdoc cref="Activity.Comment"/>
        public string Comment { get; set; }

        /// <inheritdoc cref="Activity.Hidden"/>
        [Required]
        public bool Hidden { get; set; }
    }
}
