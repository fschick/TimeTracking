using FS.FilterExpressionCreator.Mvc.Attributes;
using FS.TimeTracking.Shared.Attributes;
using FS.TimeTracking.Shared.Models.MasterData;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace FS.TimeTracking.Shared.DTOs.MasterData;

/// <inheritdoc cref="Models.MasterData.Activity"/>
[ValidationDescription]
[FilterEntity(Prefix = nameof(Activity))]
[System.Diagnostics.DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
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
    [Filter(Visible = false)]
    public Guid? ProjectId { get; set; }

    /// <inheritdoc cref="Activity.Comment"/>
    public string Comment { get; set; }

    /// <inheritdoc cref="Activity.Hidden"/>
    [Required]
    public bool Hidden { get; set; }

    [JsonIgnore]
    [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => $"{Title}";
}