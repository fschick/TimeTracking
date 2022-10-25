namespace FS.TimeTracking.Abstractions.Enums;

/// <summary>
/// Scope of a resource permission.
/// </summary>
public enum PermissionScope
{
    /// <summary>
    /// The resource cannot be accessed.
    /// </summary>
    None,

    /// <summary>
    /// The resource can be displayed.
    /// </summary>
    View,

    /// <summary>
    /// The resource ca be managed.
    /// </summary>
    Manage
}