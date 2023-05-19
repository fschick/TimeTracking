using FS.TimeTracking.Abstractions.Attributes;
using FS.TimeTracking.Abstractions.DTOs.Administration;
using FS.TimeTracking.Abstractions.DTOs.Chart;
using FS.TimeTracking.Abstractions.DTOs.MasterData;
using FS.TimeTracking.Abstractions.DTOs.TimeTracking;
using FS.TimeTracking.Abstractions.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FS.TimeTracking.Abstractions.Constants;

internal class PermissionName
{
    /// <summary>
    /// Permission for data of other users.
    /// </summary>
    [Permission(PermissionGroup.Data, 1)]
    public const string FOREIGN_DATA = "foreign-data";

    /// <summary>
    /// Permission for time sheets.
    /// </summary>
    [Permission(PermissionGroup.TimeSheet, 2, typeof(TimeSheetDto), typeof(TimeSheetGridDto))]
    public const string TIME_SHEET = "time-sheet";

    /// <summary>
    /// Permission for charts by customer.
    /// </summary>
    [Permission(PermissionGroup.Chart, 1, typeof(CustomerWorkTimeDto))]
    public const string CHARTS_BY_CUSTOMER = "charts-by-customer";

    /// <summary>
    /// Permission for charts by order.
    /// </summary>
    [Permission(PermissionGroup.Chart, 2, typeof(OrderWorkTimeDto))]
    public const string CHARTS_BY_ORDER = "charts-by-order";

    /// <summary>
    /// Permission for charts by project.
    /// </summary>
    [Permission(PermissionGroup.Chart, 3, typeof(ProjectWorkTimeDto))]
    public const string CHARTS_BY_PROJECT = "charts-by-project";

    /// <summary>
    /// Permission for charts by activity.
    /// </summary>
    [Permission(PermissionGroup.Chart, 4, typeof(ActivityWorkTimeDto))]
    public const string CHARTS_BY_ACTIVITY = "charts-by-activity";

    /// <summary>
    /// Permission for charts by issue.
    /// </summary>
    [Permission(PermissionGroup.Chart, 5, typeof(IssueWorkTimeDto))]
    public const string CHARTS_BY_ISSUE = "charts-by-issue";

    /// <summary>
    /// Permission for summary activity report.
    /// </summary>
    [Permission(PermissionGroup.Report, 1)]
    public const string REPORT_ACTIVITY_SUMMARY = "report-activity-summary";

    /// <summary>
    /// Permission for detailed activity report.
    /// </summary>
    [Permission(PermissionGroup.Report, 2)]
    public const string REPORT_ACTIVITY_DETAIL = "report-activity-detail";

    /// <summary>
    /// Permission for activity report raw data.
    /// </summary>
    [Permission(PermissionGroup.Report, 3)]
    public const string REPORT_ACTIVITY_RAW_DATA = "report-activity-raw-data";

    /// <summary>
    /// Permission for master data customers.
    /// </summary>
    [Permission(PermissionGroup.MasterData, 1, typeof(CustomerDto), typeof(CustomerGridDto))]
    public const string MASTER_DATA_CUSTOMERS = "master-data-customers";

    /// <summary>
    /// Permission for master data projects.
    /// </summary>
    [Permission(PermissionGroup.MasterData, 2, typeof(ProjectDto), typeof(ProjectGridDto))]
    public const string MASTER_DATA_PROJECTS = "master-data-projects";

    /// <summary>
    /// Permission for master data activities.
    /// </summary>
    [Permission(PermissionGroup.MasterData, 3, typeof(ActivityDto), typeof(ActivityGridDto))]
    public const string MASTER_DATA_ACTIVITIES = "master-data-activities";

    /// <summary>
    /// Permission for master data orders.
    /// </summary>
    [Permission(PermissionGroup.MasterData, 4, typeof(OrderDto), typeof(OrderGridDto))]
    public const string MASTER_DATA_ORDERS = "master-data-orders";

    /// <summary>
    /// Permission for master data holidays.
    /// </summary>
    [Permission(PermissionGroup.MasterData, 5, typeof(HolidayDto), typeof(HolidayGridDto))]
    public const string MASTER_DATA_HOLIDAYS = "master-data-holidays";

    /// <summary>
    /// Permission for users administration.
    /// </summary>
    [Permission(PermissionGroup.Administration, 1, typeof(UserDto), typeof(UserGridDto))]
    public const string ADMINISTRATION_USERS = "administration-users";

    /// <summary>
    /// Permission for administrative settings.
    /// </summary>
    [Permission(PermissionGroup.Administration, 2, typeof(SettingDto))]
    public const string ADMINISTRATION_SETTINGS = "administration-settings";

    /// <summary>
    /// Permission to maintenance data, e.g. import, export or truncate data.
    /// </summary>
    [Permission(PermissionGroup.Administration, 3)]
    public const string ADMINISTRATION_MAINTENANCE = "administration-maintenance";

    /// <summary>
    /// All role names.
    /// </summary>
    public static IEnumerable<string> All { get; } = GetAllPermissionNames();

    public static PermissionGroup ToGroup(string permissionName)
        => _permissionToGroupMap[permissionName];

    public static int ToSortOrder(string permissionName)
        => _permissionToSortOrderMap[permissionName];

    public static string FromProtectedDto(Type protectedDto)
        => _protectedTypeToPermissionMap.GetValueOrDefault(protectedDto);

    /// <summary>
    /// Permissions used to build policies for generic CRUD model services.
    /// </summary>
    /// <autogeneratedoc />
    public static string[] CrudServicePermissionNames { get; } = {
        TIME_SHEET,
        MASTER_DATA_CUSTOMERS,
        MASTER_DATA_PROJECTS,
        MASTER_DATA_ACTIVITIES,
        MASTER_DATA_ORDERS,
        MASTER_DATA_HOLIDAYS,
        ADMINISTRATION_USERS,
    };

    private static IEnumerable<string> GetAllPermissionNames()
        => typeof(PermissionName)
            .GetFields()
            .Where(field => field.IsLiteral)
            .Select(permission => (string)permission.GetValue(null))
            .ToList();

    private static readonly Dictionary<string, PermissionGroup> _permissionToGroupMap
        = typeof(PermissionName)
            .GetFields()
            .Where(field => field.IsLiteral)
            .ToDictionary(
                permission => (string)permission.GetValue(null),
                permission => permission.GetCustomAttribute<PermissionAttribute>()?.Group ?? throw new InvalidOperationException("Permission does not have a group.")
            );

    private static readonly Dictionary<string, int> _permissionToSortOrderMap
        = typeof(PermissionName)
            .GetFields()
            .Where(field => field.IsLiteral)
            .ToDictionary(
                permission => (string)permission.GetValue(null),
                permission => permission.GetCustomAttribute<PermissionAttribute>()?.SortOrder ?? throw new InvalidOperationException("Permission does not have a sort order.")
            );

    private static readonly Dictionary<Type, string> _protectedTypeToPermissionMap
        = typeof(PermissionName)
            .GetFields()
            .Where(field => field.IsLiteral)
            .SelectMany(permission =>
            {
                var permissionName = (string)permission.GetValue(null);
                return permission
                    .GetCustomAttribute<PermissionAttribute>()?.ProtectedDtos
                    .Select(protectedDto => new { ProtectedDto = protectedDto, PermissionName = permissionName });
            })
            .Where(tuple => tuple != null)
            .ToDictionary(
                tuple => tuple.ProtectedDto,
                tuple => tuple.PermissionName
            );
}