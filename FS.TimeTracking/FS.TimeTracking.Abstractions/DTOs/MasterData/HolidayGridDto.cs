using FS.TimeTracking.Abstractions.DTOs.Administration;
using FS.TimeTracking.Abstractions.Enums;
using FS.TimeTracking.Abstractions.Interfaces.DTOs;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace FS.TimeTracking.Abstractions.DTOs.MasterData;

/// <inheritdoc cref="HolidayDto"/>
[ExcludeFromCodeCoverage]
[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
public record HolidayGridDto : IIdEntityDto, IManageableDto, IUserLinkedGridDto
{
    /// <inheritdoc cref="HolidayDto.Id"/>
    [Required]
    public Guid Id { get; set; }

    /// <inheritdoc cref="HolidayDto.Title"/>
    [Required]
    public string Title { get; set; }

    /// <inheritdoc cref="HolidayDto.StartDate"/>
    [Required]
    public DateTimeOffset StartDate { get; set; }

    /// <inheritdoc cref="HolidayDto.EndDate"/>
    [Required]
    public DateTimeOffset EndDate { get; set; }

    /// <inheritdoc cref="HolidayDto.Type"/>
    [Required]
    public HolidayType Type { get; set; }

    /// <summary>
    /// The identifier of the user this entity belongs to.
    /// </summary>
    [Required]
    [JsonIgnore]
    public Guid UserId { get; set; }

    /// <inheritdoc cref="UserDto.Username"/>
    public string Username { get; set; }

    /// <inheritdoc />
    public bool? IsReadonly { get; set; }

    [JsonIgnore]
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => $"{Title} ({StartDate:d} - {EndDate:d})";
}