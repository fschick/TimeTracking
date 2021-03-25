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

        /// <inheritdoc cref="Activity.Name"/>
        [Required]
        public string Name { get; set; }

        /// <inheritdoc cref="Customer.ShortName"/>
        public string CustomerShortName { get; set; }

        /// <inheritdoc cref="Customer.CompanyName"/>
        public string CustomerCompanyName { get; set; }

        /// <inheritdoc cref="Project.Name"/>
        public string ProjectName { get; set; }

        /// <inheritdoc cref="Project.Hidden"/>
        public bool Hidden { get; set; }
    }
}
