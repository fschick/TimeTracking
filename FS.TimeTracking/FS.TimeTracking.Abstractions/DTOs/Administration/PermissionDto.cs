using FS.TimeTracking.Abstractions.Attributes;
using FS.TimeTracking.Abstractions.Enums;

namespace FS.TimeTracking.Abstractions.DTOs.Administration;

/// <summary>
/// Permission for a resource.
/// </summary>
[ValidationDescription]
public class PermissionDto
{
    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this object is manageable (insert, update, delete).
    /// </summary>
    public bool Manageable { get; set; }

    /// <summary>
    /// Gets or sets the scope.
    /// </summary>
    public PermissionScope Scope { get; set; }
}