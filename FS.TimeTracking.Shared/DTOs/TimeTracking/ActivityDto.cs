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

        /// <inheritdoc cref="Activity.Name"/>
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        /// <inheritdoc cref="Activity.CustomerId"/>
        public Guid? CustomerId { get; set; }

        /// <inheritdoc cref="Activity.ProjectId"/>
        public Guid? ProjectId { get; set; }

        /// <inheritdoc cref="Activity.Comment"/>
        public string Comment { get; set; }

        /// <inheritdoc cref="Activity.Hidden"/>
        [Required]
        public bool Hidden { get; set; }
    }
}
