namespace FS.TimeTracking.Abstractions.Constants;

/// <summary>
/// Scope of a resource permission.
/// </summary>
public class PermissionScope
{
    /// <summary>
    /// Resource cannot be accessed.
    /// </summary>
    public const string NONE = "none";

    /// <summary>
    /// Resource can be displayed.
    /// </summary>
    public const string VIEW = "view";

    /// <summary>
    /// Resource can be inserted, updated, deleted.
    /// </summary>
    public const string MANAGE = "manage";
}