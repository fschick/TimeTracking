using System.Collections.Generic;
using System.Linq;

namespace FS.TimeTracking.Abstractions.Constants;

/// <summary>
/// Authorization roles
/// </summary>
public static class RoleName
{
    /// <summary>
    /// Time sheets of other users can be updated / deleted.
    /// </summary>
    public const string FOREIGN_DATA_VIEW = $"{PermissionName.FOREIGN_DATA}-{PermissionScope.VIEW}";

    /// <summary>
    /// Time sheets can be updated / deleted.
    /// </summary>
    public const string FOREIGN_DATA_MANAGE = $"{PermissionName.FOREIGN_DATA}-{PermissionScope.MANAGE}";

    /// <summary>
    /// Time sheets can be displayed.
    /// </summary>
    public const string TIME_SHEET_VIEW = $"{PermissionName.TIME_SHEET}-{PermissionScope.VIEW}";

    /// <summary>
    /// Time sheets can be created / updated / deleted.
    /// </summary>
    public const string TIME_SHEET_MANAGE = $"{PermissionName.TIME_SHEET}-{PermissionScope.MANAGE}";

    /// <summary>
    /// Chart 'by customer' can be displayed.
    /// </summary>
    public const string CHARTS_BY_CUSTOMER_VIEW = $"{PermissionName.CHARTS_BY_CUSTOMER}-{PermissionScope.VIEW}";

    /// <summary>
    /// Chart 'by order' can be displayed.
    /// </summary>
    public const string CHARTS_BY_ORDER_VIEW = $"{PermissionName.CHARTS_BY_ORDER}-{PermissionScope.VIEW}";

    /// <summary>
    /// Chart 'by project' can be displayed.
    /// </summary>
    public const string CHARTS_BY_PROJECT_VIEW = $"{PermissionName.CHARTS_BY_PROJECT}-{PermissionScope.VIEW}";

    /// <summary>
    /// Chart 'by activity' can be displayed.
    /// </summary>
    public const string CHARTS_BY_ACTIVITY_VIEW = $"{PermissionName.CHARTS_BY_ACTIVITY}-{PermissionScope.VIEW}";

    /// <summary>
    /// Chart 'by issue' can be displayed.
    /// </summary>
    public const string CHARTS_BY_ISSUE_VIEW = $"{PermissionName.CHARTS_BY_ISSUE}-{PermissionScope.VIEW}";

    /// <summary>
    /// Activity report activity can be generated.
    /// </summary>
    public const string REPORT_ACTIVITY_SUMMARY_VIEW = $"{PermissionName.REPORT_ACTIVITY_SUMMARY}-{PermissionScope.VIEW}";

    /// <summary>
    /// Detailed activity report activity can be generated.
    /// </summary>
    public const string REPORT_ACTIVITY_DETAIL_VIEW = $"{PermissionName.REPORT_ACTIVITY_DETAIL}-{PermissionScope.VIEW}";

    /// <summary>
    /// Activity report raw data can be generated.
    /// </summary>
    public const string REPORT_ACTIVITY_RAW_DATA_VIEW = $"{PermissionName.REPORT_ACTIVITY_RAW_DATA}-{PermissionScope.VIEW}";

    /// <summary>
    /// Master data customers can be displayed.
    /// </summary>
    public const string MASTER_DATA_CUSTOMERS_VIEW = $"{PermissionName.MASTER_DATA_CUSTOMERS}-{PermissionScope.VIEW}";

    /// <summary>
    /// Master data customers can be created / updated / deleted.
    /// </summary>
    public const string MASTER_DATA_CUSTOMERS_MANAGE = $"{PermissionName.MASTER_DATA_CUSTOMERS}-{PermissionScope.MANAGE}";

    /// <summary>
    /// Master data projects can be displayed.
    /// </summary>
    public const string MASTER_DATA_PROJECTS_VIEW = $"{PermissionName.MASTER_DATA_PROJECTS}-{PermissionScope.VIEW}";

    /// <summary>
    /// Master data projects can be created / updated / deleted.
    /// </summary>
    public const string MASTER_DATA_PROJECTS_MANAGE = $"{PermissionName.MASTER_DATA_PROJECTS}-{PermissionScope.MANAGE}";

    /// <summary>
    /// Master data activities can be displayed.
    /// </summary>
    public const string MASTER_DATA_ACTIVITIES_VIEW = $"{PermissionName.MASTER_DATA_ACTIVITIES}-{PermissionScope.VIEW}";

    /// <summary>
    /// Master data activities can be created / updated / deleted.
    /// </summary>
    public const string MASTER_DATA_ACTIVITIES_MANAGE = $"{PermissionName.MASTER_DATA_ACTIVITIES}-{PermissionScope.MANAGE}";

    /// <summary>
    /// Master data orders can be displayed.
    /// </summary>
    public const string MASTER_DATA_ORDERS_VIEW = $"{PermissionName.MASTER_DATA_ORDERS}-{PermissionScope.VIEW}";

    /// <summary>
    /// Master data orders can be created / updated / deleted.
    /// </summary>
    public const string MASTER_DATA_ORDERS_MANAGE = $"{PermissionName.MASTER_DATA_ORDERS}-{PermissionScope.MANAGE}";

    /// <summary>
    /// Master data holidays can be displayed.
    /// </summary>
    public const string MASTER_DATA_HOLIDAYS_VIEW = $"{PermissionName.MASTER_DATA_HOLIDAYS}-{PermissionScope.VIEW}";

    /// <summary>
    /// Master data holidays can be created / updated / deleted.
    /// </summary>
    public const string MASTER_DATA_HOLIDAYS_MANAGE = $"{PermissionName.MASTER_DATA_HOLIDAYS}-{PermissionScope.MANAGE}";
    /// <summary>
    /// Master data settings can be displayed.
    /// </summary>
    public const string ADMINISTRATION_USERS_VIEW = $"{PermissionName.ADMINISTRATION_USERS}-{PermissionScope.VIEW}";

    /// <summary>
    /// Master data settings can be created / updated / deleted.
    /// </summary>
    public const string ADMINISTRATION_USERS_MANAGE = $"{PermissionName.ADMINISTRATION_USERS}-{PermissionScope.MANAGE}";

    /// <summary>
    /// Master data settings can be displayed.
    /// </summary>
    public const string ADMINISTRATION_SETTINGS_VIEW = $"{PermissionName.ADMINISTRATION_SETTINGS}-{PermissionScope.VIEW}";

    /// <summary>
    /// Master data settings can be created / updated / deleted.
    /// </summary>
    public const string ADMINISTRATION_SETTINGS_MANAGE = $"{PermissionName.ADMINISTRATION_SETTINGS}-{PermissionScope.MANAGE}";

    /// <summary>
    /// Data can be maintained, e.g. exported.
    /// </summary>
    public const string ADMINISTRATION_MAINTENANCE_VIEW = $"{PermissionName.ADMINISTRATION_MAINTENANCE}-{PermissionScope.VIEW}";

    /// <summary>
    /// Data can be maintained, e.g. imported or truncated.
    /// </summary>
    public const string ADMINISTRATION_MAINTENANCE_MANAGE = $"{PermissionName.ADMINISTRATION_MAINTENANCE}-{PermissionScope.MANAGE}";

    /// <summary>
    /// All role names.
    /// </summary>
    public static List<string> All { get; } = GetAllRoleNames();

    private static List<string> GetAllRoleNames()
        => typeof(RoleName)
            .GetFields()
            .Where(x => x.IsLiteral)
            .Select(x => (string)x.GetValue(null))
            .ToList();
}