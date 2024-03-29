﻿using FS.TimeTracking.Abstractions.Attributes;
using FS.TimeTracking.Abstractions.Constants;
using FS.TimeTracking.Abstractions.Enums;

namespace FS.TimeTracking.Abstractions.DTOs.Administration;

/// <summary>
/// Permission for a resource.
/// </summary>
[ValidationDescription]
public record PermissionDto
{
    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets or sets a value indicating whether this object is manageable (insert, update, delete).
    /// </summary>
    public bool Manageable { get; }

    /// <summary>
    /// Gets or sets the scope.
    /// </summary>
    public string Scope { get; set; }

    /// <summary>
    /// Gets or sets the permission group.
    /// </summary>
    public PermissionGroup Group { get; set; }

    /// <summary>
    /// Sort order.
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PermissionDto"/> class.
    /// </summary>
    /// <param name="name">Gets or sets the name.</param>
    /// <param name="manageable">Gets or sets a value indicating whether this object is manageable (insert, update, delete).</param>
    /// <param name="scope">Gets or sets the scope.</param>
    /// <autogeneratedoc />
    public PermissionDto(string name, bool manageable, string scope)
    {
        Name = name;
        Manageable = manageable;
        Scope = scope;
        Group = PermissionName.ToGroup(name);
        SortOrder = PermissionName.ToSortOrder(name);
    }
}