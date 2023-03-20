using FS.TimeTracking.Abstractions.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FS.TimeTracking.Abstractions.Attributes;

/// <summary>
/// Attribute to associate a permission group.
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public class PermissionAttribute : Attribute
{
    /// <summary>
    /// Associated permission group
    /// </summary>
    public PermissionGroup Group { get; }

    /// <summary>
    /// Sort order.
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// DTOs protected by this permission.
    /// </summary>
    public List<Type> ProtectedDtos { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PermissionAttribute"/> class.
    /// </summary>
    /// <param name="group">The permission group to associate.</param>
    /// <param name="sortOrder">Sort order.</param>
    /// <param name="protectedDtos">DTOs protected by this permission.</param>
    public PermissionAttribute(PermissionGroup group, int sortOrder, params Type[] protectedDtos)
    {
        Group = group;
        SortOrder = sortOrder;
        ProtectedDtos = protectedDtos.ToList();
    }
}
