using FS.TimeTracking.Shared.Attributes;
using FS.TimeTracking.Shared.Models.TimeTracking;
using System;
using System.ComponentModel.DataAnnotations;

namespace FS.TimeTracking.Shared.DTOs.TimeTracking
{
    /// <inheritdoc cref="Project"/>
    [ValidationDescription]
    public class ActivityListDto
    {
        /// <inheritdoc cref="Activity.Id"/>
        [Required]
        public Guid Id { get; set; }

        /// <inheritdoc cref="Activity.Title"/>
        [Required]
        public string Title { get; set; }

        /// <inheritdoc cref="Customer.Title"/>
        public string CustomerTitle { get; set; }

        /// <inheritdoc cref="Customer.CompanyName"/>
        public string CustomerCompanyName { get; set; }

        /// <inheritdoc cref="Project.Title"/>
        public string ProjectTitle { get; set; }

        /// <inheritdoc cref="Project.Hidden"/>
        public bool Hidden { get; set; }
    }
}
