using FS.TimeTracking.Abstractions.DTOs.Administration;
using FS.TimeTracking.Abstractions.Enums;
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
        new PermissionDto(PermissionNames.FOREIGN_DATA, true, PermissionScope.None),
        new PermissionDto(PermissionNames.TIME_SHEET, true, PermissionScope.None),
        new PermissionDto(PermissionNames.CHARTS_BY_CUSTOMER, false, PermissionScope.None),
        new PermissionDto(PermissionNames.CHARTS_BY_ORDER, false, PermissionScope.None),
        new PermissionDto(PermissionNames.CHARTS_BY_PROJECT, false, PermissionScope.None),
        new PermissionDto(PermissionNames.CHARTS_BY_ACTIVITY, false, PermissionScope.None),
        new PermissionDto(PermissionNames.CHARTS_BY_ISSUE, false, PermissionScope.None),
        new PermissionDto(PermissionNames.REPORT_ACTIVITY_SUMMARY, false, PermissionScope.None),
        new PermissionDto(PermissionNames.REPORT_ACTIVITY_DETAIL, false, PermissionScope.None),
        new PermissionDto(PermissionNames.REPORT_ACTIVITY_RAW_DATA, false, PermissionScope.None),
        new PermissionDto(PermissionNames.MASTER_DATA_CUSTOMERS, true, PermissionScope.None),
        new PermissionDto(PermissionNames.MASTER_DATA_PROJECTS, true, PermissionScope.None),
        new PermissionDto(PermissionNames.MASTER_DATA_ACTIVITIES, true, PermissionScope.None),
        new PermissionDto(PermissionNames.MASTER_DATA_ORDERS, true, PermissionScope.None),
        new PermissionDto(PermissionNames.MASTER_DATA_HOLIDAYS, true, PermissionScope.None),
        new PermissionDto(PermissionNames.ADMINISTRATION_USERS, true, PermissionScope.None),
        new PermissionDto(PermissionNames.ADMINISTRATION_SETTINGS, true, PermissionScope.None),
    };

    /// <summary>
    /// Default permissions for new user.
    /// </summary>
    public static List<PermissionDto> ReadPermissions => new()
    {
        new PermissionDto(PermissionNames.FOREIGN_DATA, true, PermissionScope.View),
        new PermissionDto(PermissionNames.TIME_SHEET, true, PermissionScope.View),
        new PermissionDto(PermissionNames.CHARTS_BY_CUSTOMER, false, PermissionScope.View),
        new PermissionDto(PermissionNames.CHARTS_BY_ORDER, false, PermissionScope.View),
        new PermissionDto(PermissionNames.CHARTS_BY_PROJECT, false, PermissionScope.View),
        new PermissionDto(PermissionNames.CHARTS_BY_ACTIVITY, false, PermissionScope.View),
        new PermissionDto(PermissionNames.CHARTS_BY_ISSUE, false, PermissionScope.View),
        new PermissionDto(PermissionNames.REPORT_ACTIVITY_SUMMARY, false, PermissionScope.View),
        new PermissionDto(PermissionNames.REPORT_ACTIVITY_DETAIL, false, PermissionScope.View),
        new PermissionDto(PermissionNames.REPORT_ACTIVITY_RAW_DATA, false, PermissionScope.View),
        new PermissionDto(PermissionNames.MASTER_DATA_CUSTOMERS, true, PermissionScope.View),
        new PermissionDto(PermissionNames.MASTER_DATA_PROJECTS, true, PermissionScope.View),
        new PermissionDto(PermissionNames.MASTER_DATA_ACTIVITIES, true, PermissionScope.View),
        new PermissionDto(PermissionNames.MASTER_DATA_ORDERS, true, PermissionScope.View),
        new PermissionDto(PermissionNames.MASTER_DATA_HOLIDAYS, true, PermissionScope.View),
        new PermissionDto(PermissionNames.ADMINISTRATION_USERS, true, PermissionScope.View),
        new PermissionDto(PermissionNames.ADMINISTRATION_SETTINGS, true, PermissionScope.View),
    };

    /// <summary>
    /// Default permissions for new user.
    /// </summary>
    public static List<PermissionDto> WritePermissions => new()
    {
        new PermissionDto(PermissionNames.FOREIGN_DATA, true, PermissionScope.Manage),
        new PermissionDto(PermissionNames.TIME_SHEET, true, PermissionScope.Manage),
        new PermissionDto(PermissionNames.CHARTS_BY_CUSTOMER, false, PermissionScope.View),
        new PermissionDto(PermissionNames.CHARTS_BY_ORDER, false, PermissionScope.View),
        new PermissionDto(PermissionNames.CHARTS_BY_PROJECT, false, PermissionScope.View),
        new PermissionDto(PermissionNames.CHARTS_BY_ACTIVITY, false, PermissionScope.View),
        new PermissionDto(PermissionNames.CHARTS_BY_ISSUE, false, PermissionScope.View),
        new PermissionDto(PermissionNames.REPORT_ACTIVITY_SUMMARY, false, PermissionScope.View),
        new PermissionDto(PermissionNames.REPORT_ACTIVITY_DETAIL, false, PermissionScope.View),
        new PermissionDto(PermissionNames.REPORT_ACTIVITY_RAW_DATA, false, PermissionScope.View),
        new PermissionDto(PermissionNames.MASTER_DATA_CUSTOMERS, true, PermissionScope.Manage),
        new PermissionDto(PermissionNames.MASTER_DATA_PROJECTS, true, PermissionScope.Manage),
        new PermissionDto(PermissionNames.MASTER_DATA_ACTIVITIES, true, PermissionScope.Manage),
        new PermissionDto(PermissionNames.MASTER_DATA_ORDERS, true, PermissionScope.Manage),
        new PermissionDto(PermissionNames.MASTER_DATA_HOLIDAYS, true, PermissionScope.Manage),
        new PermissionDto(PermissionNames.ADMINISTRATION_USERS, true, PermissionScope.Manage),
        new PermissionDto(PermissionNames.ADMINISTRATION_SETTINGS, true, PermissionScope.Manage),
    };
}