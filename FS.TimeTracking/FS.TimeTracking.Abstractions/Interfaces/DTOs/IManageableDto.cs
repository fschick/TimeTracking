namespace FS.TimeTracking.Abstractions.Interfaces.DTOs;

/// <summary>
/// Interface for manageable DTOs.
/// </summary>
public interface IManageableDto
{
    /// <summary>
    /// Indicating whether this entity is readonly and cannot be modified by current user.
    /// </summary>
    public bool? IsReadonly { get; set; }
}