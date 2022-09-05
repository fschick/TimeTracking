using System;

namespace FS.TimeTracking.Abstractions.Attributes;

/// <summary>
/// If <see cref="Enabled" /> is set, property validation attributes are exported from validation description services.
/// Implements the <see cref="System.Attribute" />
/// </summary>
/// <seealso cref="System.Attribute" />
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public class ValidationDescriptionAttribute : Attribute
{
    /// <summary>
    /// Indicates whether this attribute is active
    /// </summary>
    public bool Enabled { get; set; } = true;
}