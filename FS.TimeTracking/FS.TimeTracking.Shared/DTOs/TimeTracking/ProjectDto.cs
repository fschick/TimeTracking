using FS.FilterExpressionCreator.Mvc.Attributes;
using FS.TimeTracking.Shared.Attributes;
using FS.TimeTracking.Shared.Models.TimeTracking;
using System;
using System.ComponentModel.DataAnnotations;

namespace FS.TimeTracking.Shared.DTOs.TimeTracking
{
    /// <inheritdoc cref="Project"/>
    [ValidationDescription]
    [FilterEntity(Prefix = nameof(Project))]
    public class ProjectDto
    {
        /// <inheritdoc cref="Project.Id"/>
        [Required]
        [Filter(Visible = false)]
        public Guid Id { get; set; }

        /// <inheritdoc cref="Project.Title"/>
        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        /// <inheritdoc cref="Project.CustomerId"/>
        [Required]
        public Guid CustomerId { get; set; }

        /// <inheritdoc cref="Project.Comment"/>
        public string Comment { get; set; }

        /// <inheritdoc cref="Project.Hidden"/>
        [Required]
        public bool Hidden { get; set; }
    }
}
