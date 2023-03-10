using FS.TimeTracking.Abstractions.DTOs.Administration;
using System.Collections.Generic;

namespace FS.TimeTracking.Abstractions.Constants;

/// <summary>
/// All permissions used to manage resources.
/// </summary>
public static class DefaultPermissions
{
    /// <summary>
    /// Default permissions for new user.
    /// </summary>
    public static List<PermissionDto> NoPermissions => new()
    {
        new PermissionDto(PermissionName.FOREIGN_DATA, true, PermissionScope.NONE),
        new PermissionDto(PermissionName.TIME_SHEET, true, PermissionScope.NONE),
        new PermissionDto(PermissionName.CHARTS_BY_CUSTOMER, false, PermissionScope.NONE),
        new PermissionDto(PermissionName.CHARTS_BY_ORDER, false, PermissionScope.NONE),
        new PermissionDto(PermissionName.CHARTS_BY_PROJECT, false, PermissionScope.NONE),
        new PermissionDto(PermissionName.CHARTS_BY_ACTIVITY, false, PermissionScope.NONE),
        new PermissionDto(PermissionName.CHARTS_BY_ISSUE, false, PermissionScope.NONE),
        new PermissionDto(PermissionName.REPORT_ACTIVITY_SUMMARY, false, PermissionScope.NONE),
        new PermissionDto(PermissionName.REPORT_ACTIVITY_DETAIL, false, PermissionScope.NONE),
        new PermissionDto(PermissionName.REPORT_ACTIVITY_RAW_DATA, false, PermissionScope.NONE),
        new PermissionDto(PermissionName.MASTER_DATA_CUSTOMERS, true, PermissionScope.NONE),
        new PermissionDto(PermissionName.MASTER_DATA_PROJECTS, true, PermissionScope.NONE),
        new PermissionDto(PermissionName.MASTER_DATA_ACTIVITIES, true, PermissionScope.NONE),
        new PermissionDto(PermissionName.MASTER_DATA_ORDERS, true, PermissionScope.NONE),
        new PermissionDto(PermissionName.MASTER_DATA_HOLIDAYS, true, PermissionScope.NONE),
        new PermissionDto(PermissionName.ADMINISTRATION_USERS, true, PermissionScope.NONE),
        new PermissionDto(PermissionName.ADMINISTRATION_SETTINGS, true, PermissionScope.NONE),
    };

    /// <summary>
    /// Default permissions for new user.
    /// </summary>
    public static List<PermissionDto> ReadPermissions => new()
    {
        new PermissionDto(PermissionName.FOREIGN_DATA, true, PermissionScope.VIEW),
        new PermissionDto(PermissionName.TIME_SHEET, true, PermissionScope.VIEW),
        new PermissionDto(PermissionName.CHARTS_BY_CUSTOMER, false, PermissionScope.VIEW),
        new PermissionDto(PermissionName.CHARTS_BY_ORDER, false, PermissionScope.VIEW),
        new PermissionDto(PermissionName.CHARTS_BY_PROJECT, false, PermissionScope.VIEW),
        new PermissionDto(PermissionName.CHARTS_BY_ACTIVITY, false, PermissionScope.VIEW),
        new PermissionDto(PermissionName.CHARTS_BY_ISSUE, false, PermissionScope.VIEW),
        new PermissionDto(PermissionName.REPORT_ACTIVITY_SUMMARY, false, PermissionScope.VIEW),
        new PermissionDto(PermissionName.REPORT_ACTIVITY_DETAIL, false, PermissionScope.VIEW),
        new PermissionDto(PermissionName.REPORT_ACTIVITY_RAW_DATA, false, PermissionScope.VIEW),
        new PermissionDto(PermissionName.MASTER_DATA_CUSTOMERS, true, PermissionScope.VIEW),
        new PermissionDto(PermissionName.MASTER_DATA_PROJECTS, true, PermissionScope.VIEW),
        new PermissionDto(PermissionName.MASTER_DATA_ACTIVITIES, true, PermissionScope.VIEW),
        new PermissionDto(PermissionName.MASTER_DATA_ORDERS, true, PermissionScope.VIEW),
        new PermissionDto(PermissionName.MASTER_DATA_HOLIDAYS, true, PermissionScope.VIEW),
        new PermissionDto(PermissionName.ADMINISTRATION_USERS, true, PermissionScope.VIEW),
        new PermissionDto(PermissionName.ADMINISTRATION_SETTINGS, true, PermissionScope.VIEW),
    };

    /// <summary>
    /// Default permissions for new user.
    /// </summary>
    public static List<PermissionDto> WritePermissions => new()
    {
        new PermissionDto(PermissionName.FOREIGN_DATA, true, PermissionScope.MANAGE),
        new PermissionDto(PermissionName.TIME_SHEET, true, PermissionScope.MANAGE),
        new PermissionDto(PermissionName.CHARTS_BY_CUSTOMER, false, PermissionScope.VIEW),
        new PermissionDto(PermissionName.CHARTS_BY_ORDER, false, PermissionScope.VIEW),
        new PermissionDto(PermissionName.CHARTS_BY_PROJECT, false, PermissionScope.VIEW),
        new PermissionDto(PermissionName.CHARTS_BY_ACTIVITY, false, PermissionScope.VIEW),
        new PermissionDto(PermissionName.CHARTS_BY_ISSUE, false, PermissionScope.VIEW),
        new PermissionDto(PermissionName.REPORT_ACTIVITY_SUMMARY, false, PermissionScope.VIEW),
        new PermissionDto(PermissionName.REPORT_ACTIVITY_DETAIL, false, PermissionScope.VIEW),
        new PermissionDto(PermissionName.REPORT_ACTIVITY_RAW_DATA, false, PermissionScope.VIEW),
        new PermissionDto(PermissionName.MASTER_DATA_CUSTOMERS, true, PermissionScope.MANAGE),
        new PermissionDto(PermissionName.MASTER_DATA_PROJECTS, true, PermissionScope.MANAGE),
        new PermissionDto(PermissionName.MASTER_DATA_ACTIVITIES, true, PermissionScope.MANAGE),
        new PermissionDto(PermissionName.MASTER_DATA_ORDERS, true, PermissionScope.MANAGE),
        new PermissionDto(PermissionName.MASTER_DATA_HOLIDAYS, true, PermissionScope.MANAGE),
        new PermissionDto(PermissionName.ADMINISTRATION_USERS, true, PermissionScope.MANAGE),
        new PermissionDto(PermissionName.ADMINISTRATION_SETTINGS, true, PermissionScope.MANAGE),
    };
}