using FS.FilterExpressionCreator.Mvc.Attributes;
using FS.TimeTracking.Shared.Attributes;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using FS.TimeTracking.Shared.Models.Application.TimeTracking;

namespace FS.TimeTracking.Shared.DTOs.TimeTracking;

/// <inheritdoc cref="TimeSheet"/>
[ValidationDescription]
[FilterEntity(Prefix = nameof(TimeSheet))]
[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
public class TimeSheetDto
{
    /// <inheritdoc cref="TimeSheet.Id"/>
    [Required]
    public Guid Id { get; set; }

    /// <inheritdoc cref="TimeSheet.ProjectId"/>
    [Required]
    [Filter(Visible = false)]
    public Guid ProjectId { get; set; }

    /// <inheritdoc cref="TimeSheet.ActivityId"/>
    [Required]
    [Filter(Visible = false)]
    public Guid ActivityId { get; set; }

    /// <inheritdoc cref="TimeSheet.OrderId"/>
    [Filter(Visible = false)]
    public Guid? OrderId { get; set; }

    /// <inheritdoc cref="TimeSheet.Issue"/>
    public string Issue { get; set; }

    /// <inheritdoc cref="TimeSheet.StartDate"/>
    [Required]
    public DateTimeOffset StartDate { get; set; }

    /// <inheritdoc cref="TimeSheet.EndDate"/>
    [CompareTo(Models.Shared.ComparisonType.GreaterThan, nameof(StartDate))]
    public DateTimeOffset? EndDate { get; set; }

    /// <inheritdoc cref="TimeSheet.Billable"/>
    [Required]
    public bool Billable { get; set; }

    /// <inheritdoc cref="TimeSheet.Comment"/>
    public string Comment { get; set; }

    [JsonIgnore]
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => $"{StartDate:d} - {EndDate:d}";
}