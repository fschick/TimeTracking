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
    public static List<PermissionDto> Value => new()
    {
        CreatePermission(PermissionNames.TIME_SHEET, true),
        CreatePermission(PermissionNames.CHARTS_BY_CUSTOMER, false),
        CreatePermission(PermissionNames.CHARTS_BY_ORDER, false),
        CreatePermission(PermissionNames.CHARTS_BY_PROJECT, false),
        CreatePermission(PermissionNames.CHARTS_BY_ACTIVITY, false),
        CreatePermission(PermissionNames.CHARTS_BY_ISSUE, false),
        CreatePermission(PermissionNames.REPORT_ACTIVITY_SUMMARY, false),
        CreatePermission(PermissionNames.REPORT_ACTIVITY_DETAIL, false),
        CreatePermission(PermissionNames.REPORT_ACTIVITY_RAW_DATA, false),
        CreatePermission(PermissionNames.MASTER_DATA_CUSTOMERS, true),
        CreatePermission(PermissionNames.MASTER_DATA_PROJECTS, true),
        CreatePermission(PermissionNames.MASTER_DATA_ACTIVITIES, true),
        CreatePermission(PermissionNames.MASTER_DATA_ORDERS, true),
        CreatePermission(PermissionNames.MASTER_DATA_HOLIDAYS, true),
        CreatePermission(PermissionNames.ADMINISTRATION_USERS, true),
        CreatePermission(PermissionNames.ADMINISTRATION_SETTINGS, true),
    };

    private static PermissionDto CreatePermission(string name, bool manageable)
    {
        return new PermissionDto
        {
            Name = name,
            Manageable = manageable,
            Scope = PermissionScope.None,
        };
    }
}