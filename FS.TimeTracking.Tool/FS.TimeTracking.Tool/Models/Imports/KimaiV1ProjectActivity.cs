using System.ComponentModel.DataAnnotations;

namespace FS.TimeTracking.Tool.Models.Imports;

/// <summary>
/// Project to activity relation
/// </summary>
public class KimaiV1ProjectActivity
{
    /// <summary>
    /// Gets the unique activity identifier.
    /// </summary>
    [Required]
    public int ActivityId { get; set; }

    /// <summary>
    /// Gets the activity.
    /// </summary>
    public KimaiV1Activity Activity { get; set; }

    /// <summary>
    /// Gets the unique project identifier.
    /// </summary>
    [Required]
    public int ProjectId { get; set; }

    /// <summary>
    /// Gets the project.
    /// </summary>
    public KimaiV1Project Project { get; set; }
}