using FS.FilterExpressionCreator.Mvc.Attributes;
using FS.TimeTracking.Shared.Attributes;
using FS.TimeTracking.Shared.Models.TimeTracking;
using System;
using System.ComponentModel.DataAnnotations;
using FS.TimeTracking.Shared.Models.MasterData;

namespace FS.TimeTracking.Shared.DTOs.TimeTracking;

/// <inheritdoc cref="Activity"/>
[ValidationDescription]
[FilterEntity(Prefix = nameof(Activity))]
public class ActivityDto
{
    /// <inheritdoc cref="Activity.Id"/>
    [Required]
    [Filter(Visible = false)]
    public Guid Id { get; set; }

    /// <inheritdoc cref="Activity.Title"/>
    [Required]
    [StringLength(100)]
    public string Title { get; set; }

    /// <inheritdoc cref="Activity.ProjectId"/>
    [Filter(Visible = false)]
    public Guid? ProjectId { get; set; }

    /// <inheritdoc cref="Activity.Comment"/>
    public string Comment { get; set; }

    /// <inheritdoc cref="Activity.Hidden"/>
    [Required]
    public bool Hidden { get; set; }
}