using FS.TimeTracking.Abstractions.DTOs.Administration;
using FS.TimeTracking.Abstractions.DTOs.MasterData;
using FS.TimeTracking.Abstractions.Interfaces.DTOs;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace FS.TimeTracking.Abstractions.DTOs.TimeTracking;

/// <inheritdoc cref="TimeSheetDto"/>
[ExcludeFromCodeCoverage]
[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
public record TimeSheetGridDto : IIdEntityDto, IManageableDto, IUserLinkedGridDto, ICustomerLinkedDto
{
    /// <inheritdoc cref="TimeSheetDto.Id"/>
    [Required]
    public Guid Id { get; set; }

    /// <inheritdoc cref="TimeSheetDto.StartDate"/>
    [Required]
    public DateTimeOffset StartDate { get; set; }

    /// <inheritdoc cref="TimeSheetDto.EndDate"/>
    public DateTimeOffset? EndDate { get; set; }

    /// <summary>
    /// Total working time for this sheet.
    /// </summary>
    public TimeSpan? Duration { get; set; }

    /// <inheritdoc cref="TimeSheetDto.Issue"/>
    public string Issue { get; set; }

    /// <inheritdoc cref="TimeSheetDto.Comment"/>
    public string Comment { get; set; }

    /// <inheritdoc />
    [JsonIgnore]
    public Guid? CustomerId { get; set; }

    /// <inheritdoc cref="CustomerDto.Title"/>
    public string CustomerTitle { get; set; }

    /// <inheritdoc cref="ProjectDto.Title"/>
    public string ProjectTitle { get; set; }

    /// <inheritdoc cref="ActivityDto.Title"/>
    public string ActivityTitle { get; set; }

    /// <inheritdoc cref="OrderDto.Title"/>
    public string OrderTitle { get; set; }

    /// <inheritdoc cref="UserDto.Id"/>
    [Required]
    [JsonIgnore]
    public Guid UserId { get; set; }

    /// <inheritdoc cref="UserDto.Username"/>
    public string Username { get; set; }

    /// <inheritdoc cref="TimeSheetDto.Billable"/>
    [Required]
    public bool Billable { get; set; }

    /// <inheritdoc />
    public bool? IsReadonly { get; set; }

    [JsonIgnore]
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay =>
        $"{StartDate:dd.MM.yyyy HH:mm} - {EndDate:dd.MM.yyyy HH:mm}"
        + (CustomerTitle != null ? $", {CustomerTitle}" : string.Empty)
        + (ActivityTitle != null ? $", {ActivityTitle}" : string.Empty);
}