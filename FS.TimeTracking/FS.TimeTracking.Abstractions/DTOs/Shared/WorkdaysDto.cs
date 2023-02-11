using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace FS.TimeTracking.Abstractions.DTOs.Shared;

/// <summary>
/// Workdays.
/// </summary>
[ExcludeFromCodeCoverage]
public record WorkdaysDto
{
    /// <summary>
    /// Public workdays per user.
    /// </summary>
    public Dictionary<Guid, List<DateTime>> PublicWorkdays { get; set; }

    /// <summary>
    /// Personal workdays per user (taking into account individual vacation).
    /// </summary>
    public Dictionary<Guid, List<DateTime>> PersonalWorkdays { get; set; }

    /// <summary>
    /// Public workdays for all users.
    /// </summary>
    public List<DateTime> PublicWorkdaysOfAllUsers
        => PublicWorkdays
            .SelectMany(x => x.Value)
            .ToList();

    /// <summary>
    /// Personal workdays for all users.
    /// </summary>
    public List<DateTime> PersonalWorkdaysOfAllUsers
        => PersonalWorkdays
            .SelectMany(x => x.Value)
            .ToList();
}