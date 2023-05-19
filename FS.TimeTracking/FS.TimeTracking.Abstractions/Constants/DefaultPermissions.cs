using FS.TimeTracking.Abstractions.DTOs.Administration;
using System.Collections.Generic;
using System.Linq;

namespace FS.TimeTracking.Abstractions.Constants;

/// <summary>
/// All permissions used to manage resources.
/// </summary>
public static class DefaultPermissions
{
    /// <summary>
    /// Default permissions for new user.
    /// </summary>
    public static List<PermissionDto> NoPermissions => new List<PermissionDto>
        {
            new(PermissionName.FOREIGN_DATA, true, PermissionScope.NONE),
            new(PermissionName.TIME_SHEET, true, PermissionScope.NONE),
            new(PermissionName.CHARTS_BY_CUSTOMER, false, PermissionScope.NONE),
            new(PermissionName.CHARTS_BY_ORDER, false, PermissionScope.NONE),
            new(PermissionName.CHARTS_BY_PROJECT, false, PermissionScope.NONE),
            new(PermissionName.CHARTS_BY_ACTIVITY, false, PermissionScope.NONE),
            new(PermissionName.CHARTS_BY_ISSUE, false, PermissionScope.NONE),
            new(PermissionName.REPORT_ACTIVITY_SUMMARY, false, PermissionScope.NONE),
            new(PermissionName.REPORT_ACTIVITY_DETAIL, false, PermissionScope.NONE),
            new(PermissionName.REPORT_ACTIVITY_RAW_DATA, false, PermissionScope.NONE),
            new(PermissionName.MASTER_DATA_CUSTOMERS, true, PermissionScope.NONE),
            new(PermissionName.MASTER_DATA_PROJECTS, true, PermissionScope.NONE),
            new(PermissionName.MASTER_DATA_ACTIVITIES, true, PermissionScope.NONE),
            new(PermissionName.MASTER_DATA_ORDERS, true, PermissionScope.NONE),
            new(PermissionName.MASTER_DATA_HOLIDAYS, true, PermissionScope.NONE),
            new(PermissionName.ADMINISTRATION_USERS, true, PermissionScope.NONE),
            new(PermissionName.ADMINISTRATION_SETTINGS, true, PermissionScope.NONE),
            new(PermissionName.ADMINISTRATION_MAINTENANCE, true, PermissionScope.NONE),
        }
        .SortByGroupAndSortOrder();

    /// <summary>
    /// Default permissions for new user.
    /// </summary>
    public static List<PermissionDto> ReadPermissions => new List<PermissionDto>
        {
            new(PermissionName.FOREIGN_DATA, true, PermissionScope.VIEW),
            new(PermissionName.TIME_SHEET, true, PermissionScope.VIEW),
            new(PermissionName.CHARTS_BY_CUSTOMER, false, PermissionScope.VIEW),
            new(PermissionName.CHARTS_BY_ORDER, false, PermissionScope.VIEW),
            new(PermissionName.CHARTS_BY_PROJECT, false, PermissionScope.VIEW),
            new(PermissionName.CHARTS_BY_ACTIVITY, false, PermissionScope.VIEW),
            new(PermissionName.CHARTS_BY_ISSUE, false, PermissionScope.VIEW),
            new(PermissionName.REPORT_ACTIVITY_SUMMARY, false, PermissionScope.VIEW),
            new(PermissionName.REPORT_ACTIVITY_DETAIL, false, PermissionScope.VIEW),
            new(PermissionName.REPORT_ACTIVITY_RAW_DATA, false, PermissionScope.VIEW),
            new(PermissionName.MASTER_DATA_CUSTOMERS, true, PermissionScope.VIEW),
            new(PermissionName.MASTER_DATA_PROJECTS, true, PermissionScope.VIEW),
            new(PermissionName.MASTER_DATA_ACTIVITIES, true, PermissionScope.VIEW),
            new(PermissionName.MASTER_DATA_ORDERS, true, PermissionScope.VIEW),
            new(PermissionName.MASTER_DATA_HOLIDAYS, true, PermissionScope.VIEW),
            new(PermissionName.ADMINISTRATION_USERS, true, PermissionScope.VIEW),
            new(PermissionName.ADMINISTRATION_SETTINGS, true, PermissionScope.VIEW),
            new(PermissionName.ADMINISTRATION_MAINTENANCE, true, PermissionScope.VIEW),
        }
        .SortByGroupAndSortOrder();

    /// <summary>
    /// Default permissions for new user.
    /// </summary>
    public static List<PermissionDto> WritePermissions => new List<PermissionDto>
        {
            new(PermissionName.FOREIGN_DATA, true, PermissionScope.MANAGE),
            new(PermissionName.TIME_SHEET, true, PermissionScope.MANAGE),
            new(PermissionName.CHARTS_BY_CUSTOMER, false, PermissionScope.VIEW),
            new(PermissionName.CHARTS_BY_ORDER, false, PermissionScope.VIEW),
            new(PermissionName.CHARTS_BY_PROJECT, false, PermissionScope.VIEW),
            new(PermissionName.CHARTS_BY_ACTIVITY, false, PermissionScope.VIEW),
            new(PermissionName.CHARTS_BY_ISSUE, false, PermissionScope.VIEW),
            new(PermissionName.REPORT_ACTIVITY_SUMMARY, false, PermissionScope.VIEW),
            new(PermissionName.REPORT_ACTIVITY_DETAIL, false, PermissionScope.VIEW),
            new(PermissionName.REPORT_ACTIVITY_RAW_DATA, false, PermissionScope.VIEW),
            new(PermissionName.MASTER_DATA_CUSTOMERS, true, PermissionScope.MANAGE),
            new(PermissionName.MASTER_DATA_PROJECTS, true, PermissionScope.MANAGE),
            new(PermissionName.MASTER_DATA_ACTIVITIES, true, PermissionScope.MANAGE),
            new(PermissionName.MASTER_DATA_ORDERS, true, PermissionScope.MANAGE),
            new(PermissionName.MASTER_DATA_HOLIDAYS, true, PermissionScope.MANAGE),
            new(PermissionName.ADMINISTRATION_USERS, true, PermissionScope.MANAGE),
            new(PermissionName.ADMINISTRATION_SETTINGS, true, PermissionScope.MANAGE),
            new(PermissionName.ADMINISTRATION_MAINTENANCE, true, PermissionScope.MANAGE),
        }
        .SortByGroupAndSortOrder();

    private static List<PermissionDto> SortByGroupAndSortOrder(this IEnumerable<PermissionDto> permissions)
        => permissions.OrderBy(p => p.Group).ThenBy(p => p.SortOrder).ToList();
}