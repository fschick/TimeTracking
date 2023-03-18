using FS.TimeTracking.Abstractions.Interfaces.DTOs;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FS.TimeTracking.Abstractions.DTOs.Administration;

/// <inheritdoc cref="UserDto"/>
[ExcludeFromCodeCoverage]
public record UserGridDto : IIdEntityDto, IManageableDto
{
    /// <inheritdoc cref="UserDto.Id"/>
    [Required]
    public Guid Id { get; set; }

    /// <inheritdoc cref="UserDto.Username"/>
    [Required]
    public string Username { get; set; }

    /// <inheritdoc cref="UserDto.FirstName"/>
    public string FirstName { get; set; }

    /// <inheritdoc cref="UserDto.LastName"/>
    public string LastName { get; set; }

    /// <inheritdoc cref="UserDto.Email"/>
    public string Email { get; set; }

    /// <inheritdoc cref="UserDto.Enabled"/>
    public bool Enabled { get; set; }

    /// <inheritdoc />
    public bool? IsReadonly { get; set; }
}