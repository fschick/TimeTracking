using FS.TimeTracking.Abstractions.Attributes;

namespace FS.TimeTracking.Abstractions.Enums;

/// <summary>
/// Enum to describe relation between validated properties of <see cref="CompareToAttribute"/>
/// </summary>
public enum ComparisonType
{
    /// <summary>
    /// Properties are equal
    /// </summary>
    Equal,

    /// <summary>
    /// Properties are not equal
    /// </summary>
    NotEqual,

    /// <summary>
    /// Validated property is less than other property
    /// </summary>
    LessThan,

    /// <summary>
    /// Validated property is less than or equal to other property
    /// </summary>
    LessThanOrEqual,

    /// <summary>
    /// Validated property is greater than other property
    /// </summary>
    GreaterThan,

    /// <summary>
    /// Validated property is greater than or equal to other property
    /// </summary>
    GreaterThanOrEqual,
}