using FS.TimeTracking.Abstractions.Enums;
using System;

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
    /// Initializes a new instance of the <see cref="PermissionAttribute"/> class.
    /// </summary>
    /// <param name="group">The permission group to associate.</param>
    /// <param name="sortOrder">Sort order.</param>
    public PermissionAttribute(PermissionGroup group, int sortOrder)
    {
        Group = group;
        SortOrder = sortOrder;
    }
}
