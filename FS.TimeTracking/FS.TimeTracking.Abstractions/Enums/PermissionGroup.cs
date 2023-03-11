using System.Runtime.Serialization;

namespace FS.TimeTracking.Abstractions.Enums;

/// <summary>
/// Values that represent permission groups.
/// </summary>
public enum PermissionGroup
{
    /// <summary>
    /// Common data related permissions.
    /// </summary>
    Data,

    /// <summary>
    /// Time sheet related permissions.
    /// </summary>
    [EnumMember(Value = "time-sheet")]
    TimeSheet,

    /// <summary>
    /// Charts related permissions.
    /// </summary>
    Chart,

    /// <summary>
    /// Master data related permissions.
    /// </summary>
    [EnumMember(Value = "master-data")]
    MasterData,

    /// <summary>
    /// Reporting related permissions.
    /// </summary>
    Report,

    /// <summary>
    /// Administration related permissions.
    /// </summary>
    Administration,
}