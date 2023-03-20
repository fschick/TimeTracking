using FS.FilterExpressionCreator.Abstractions.Attributes;
using FS.TimeTracking.Abstractions.Attributes;
using FS.TimeTracking.Abstractions.Interfaces.DTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FS.TimeTracking.Abstractions.DTOs.Administration;

/// <summary>
/// User
/// </summary>
[ValidationDescription]
[FilterEntity(Prefix = "User")]
[ExcludeFromCodeCoverage]
public record UserDto : IIdEntityDto, IManageableDto
{
    /// <summary>
    /// The unique identifier of the entity.
    /// </summary>
    [Required]
    public Guid Id { get; set; }

    /// <summary>
    /// The username.
    /// </summary>
    [Required]
    public string Username { get; set; }

    /// <summary>
    /// The password for this user.
    /// </summary>
    [Filter(Visible = false)]
    public string Password { get; set; }

    /// <summary>
    /// The person's first name.
    /// </summary>
    public string FirstName { get; set; }

    /// <summary>
    /// The person's last name.
    /// </summary>
    public string LastName { get; set; }

    /// <summary>
    /// The user's email.
    /// </summary>
    [EmailAddress]
    public string Email { get; set; }

    /// <summary>
    /// Indicates whether this user is enabled.
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Restrict manageable data to customers from this list.
    /// </summary>
    public List<Guid> RestrictToCustomerIds { get; set; } = new();

    /// <summary>
    /// Permissions of the user.
    /// </summary>
    public List<PermissionDto> Permissions { get; set; }

    /// <inheritdoc />
    [Required]
    public bool IsReadonly { get; set; }
}