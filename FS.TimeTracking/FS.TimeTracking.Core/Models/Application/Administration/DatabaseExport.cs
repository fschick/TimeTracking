using FS.TimeTracking.Core.Models.Application.MasterData;
using FS.TimeTracking.Core.Models.Application.TimeTracking;
using System.Collections.Generic;

namespace FS.TimeTracking.Core.Models.Application.Administration;

/// <summary>
/// Database export.
/// </summary>
public class DatabaseExport
{
    /// <summary>
    /// Settings
    /// </summary>
    public List<Setting> Settings { get; init; }

    /// <summary>
    /// Holidays
    /// </summary>
    public List<Holiday> Holidays { get; init; }

    /// <summary>
    /// Customers
    /// </summary>
    public List<Customer> Customers { get; init; }

    /// <summary>
    /// Projects
    /// </summary>
    public List<Project> Projects { get; init; }

    /// <summary>
    /// Activities
    /// </summary>
    public List<Activity> Activities { get; init; }

    /// <summary>
    /// Orders
    /// </summary>
    public List<Order> Orders { get; init; }

    /// <summary>
    /// TimeSheets
    /// </summary>
    public List<TimeSheet> TimeSheets { get; init; }

    /// <summary>
    /// Migrations
    /// </summary>
    public string DatabaseModelHash { get; init; }
}