using System.Collections.Generic;
using System.Linq;

namespace FS.TimeTracking.Abstractions.Constants;

/// <summary>
/// Authorization roles
/// </summary>
public static class RoleNames
{
    /// <summary>
    /// Time sheets of other users can be updated / deleted.
    /// </summary>
    public const string FOREIGN_DATA_VIEW = $"{PermissionNames.FOREIGN_DATA}-{ScopeNames.VIEW}";

    /// <summary>
    /// Time sheets can be updated / deleted.
    /// </summary>
    public const string FOREIGN_DATA_MANAGE = $"{PermissionNames.FOREIGN_DATA}-{ScopeNames.MANAGE}";

    /// <summary>
    /// Time sheets can be displayed.
    /// </summary>
    public const string TIME_SHEET_VIEW = $"{PermissionNames.TIME_SHEET}-{ScopeNames.VIEW}";

    /// <summary>
    /// Time sheets can be created / updated / deleted.
    /// </summary>
    public const string TIME_SHEET_MANAGE = $"{PermissionNames.TIME_SHEET}-{ScopeNames.MANAGE}";

    /// <summary>
    /// Chart 'by customer' can be displayed.
    /// </summary>
    public const string CHARTS_BY_CUSTOMER_VIEW = $"{PermissionNames.CHARTS_BY_CUSTOMER}-{ScopeNames.VIEW}";

    /// <summary>
    /// Chart 'by order' can be displayed.
    /// </summary>
    public const string CHARTS_BY_ORDER_VIEW = $"{PermissionNames.CHARTS_BY_ORDER}-{ScopeNames.VIEW}";

    /// <summary>
    /// Chart 'by project' can be displayed.
    /// </summary>
    public const string CHARTS_BY_PROJECT_VIEW = $"{PermissionNames.CHARTS_BY_PROJECT}-{ScopeNames.VIEW}";

    /// <summary>
    /// Chart 'by activity' can be displayed.
    /// </summary>
    public const string CHARTS_BY_ACTIVITY_VIEW = $"{PermissionNames.CHARTS_BY_ACTIVITY}-{ScopeNames.VIEW}";

    /// <summary>
    /// Chart 'by issue' can be displayed.
    /// </summary>
    public const string CHARTS_BY_ISSUE_VIEW = $"{PermissionNames.CHARTS_BY_ISSUE}-{ScopeNames.VIEW}";

    /// <summary>
    /// Activity report activity can be generated.
    /// </summary>
    public const string REPORT_ACTIVITY_SUMMARY_VIEW = $"{PermissionNames.REPORT_ACTIVITY_SUMMARY}-{ScopeNames.VIEW}";

    /// <summary>
    /// Detailed activity report activity can be generated.
    /// </summary>
    public const string REPORT_ACTIVITY_DETAIL_VIEW = $"{PermissionNames.REPORT_ACTIVITY_DETAIL}-{ScopeNames.VIEW}";

    /// <summary>
    /// Activity report raw data can be generated.
    /// </summary>
    public const string REPORT_ACTIVITY_RAW_DATA_VIEW = $"{PermissionNames.REPORT_ACTIVITY_RAW_DATA}-{ScopeNames.VIEW}";

    /// <summary>
    /// Master data customers can be displayed.
    /// </summary>
    public const string MASTER_DATA_CUSTOMERS_VIEW = $"{PermissionNames.MASTER_DATA_CUSTOMERS}-{ScopeNames.VIEW}";

    /// <summary>
    /// Master data customers can be created / updated / deleted.
    /// </summary>
    public const string MASTER_DATA_CUSTOMERS_MANAGE = $"{PermissionNames.MASTER_DATA_CUSTOMERS}-{ScopeNames.MANAGE}";

    /// <summary>
    /// Master data projects can be displayed.
    /// </summary>
    public const string MASTER_DATA_PROJECTS_VIEW = $"{PermissionNames.MASTER_DATA_PROJECTS}-{ScopeNames.VIEW}";

    /// <summary>
    /// Master data projects can be created / updated / deleted.
    /// </summary>
    public const string MASTER_DATA_PROJECTS_MANAGE = $"{PermissionNames.MASTER_DATA_PROJECTS}-{ScopeNames.MANAGE}";

    /// <summary>
    /// Master data activities can be displayed.
    /// </summary>
    public const string MASTER_DATA_ACTIVITIES_VIEW = $"{PermissionNames.MASTER_DATA_ACTIVITIES}-{ScopeNames.VIEW}";

    /// <summary>
    /// Master data activities can be created / updated / deleted.
    /// </summary>
    public const string MASTER_DATA_ACTIVITIES_MANAGE = $"{PermissionNames.MASTER_DATA_ACTIVITIES}-{ScopeNames.MANAGE}";

    /// <summary>
    /// Master data orders can be displayed.
    /// </summary>
    public const string MASTER_DATA_ORDERS_VIEW = $"{PermissionNames.MASTER_DATA_ORDERS}-{ScopeNames.VIEW}";

    /// <summary>
    /// Master data orders can be created / updated / deleted.
    /// </summary>
    public const string MASTER_DATA_ORDERS_MANAGE = $"{PermissionNames.MASTER_DATA_ORDERS}-{ScopeNames.MANAGE}";

    /// <summary>
    /// Master data holidays can be displayed.
    /// </summary>
    public const string MASTER_DATA_HOLIDAYS_VIEW = $"{PermissionNames.MASTER_DATA_HOLIDAYS}-{ScopeNames.VIEW}";

    /// <summary>
    /// Master data holidays can be created / updated / deleted.
    /// </summary>
    public const string MASTER_DATA_HOLIDAYS_MANAGE = $"{PermissionNames.MASTER_DATA_HOLIDAYS}-{ScopeNames.MANAGE}";
    /// <summary>
    /// Master data settings can be displayed.
    /// </summary>
    public const string ADMINISTRATION_USERS_VIEW = $"{PermissionNames.ADMINISTRATION_USERS}-{ScopeNames.VIEW}";

    /// <summary>
    /// Master data settings can be created / updated / deleted.
    /// </summary>
    public const string ADMINISTRATION_USERS_MANAGE = $"{PermissionNames.ADMINISTRATION_USERS}-{ScopeNames.MANAGE}";

    /// <summary>
    /// Master data settings can be displayed.
    /// </summary>
    public const string ADMINISTRATION_SETTINGS_VIEW = $"{PermissionNames.ADMINISTRATION_SETTINGS}-{ScopeNames.VIEW}";

    /// <summary>
    /// Master data settings can be created / updated / deleted.
    /// </summary>
    public const string ADMINISTRATION_SETTINGS_MANAGE = $"{PermissionNames.ADMINISTRATION_SETTINGS}-{ScopeNames.MANAGE}";

    /// <summary>
    /// All role names.
    /// </summary>
    public static List<string> All { get; } = GetAllRoleNames();

    private static List<string> GetAllRoleNames()
        => typeof(RoleNames)
            .GetFields()
            .Where(x => x.IsLiteral)
            .Select(x => (string)x.GetValue(null))
            .ToList();
}