using FS.TimeTracking.Abstractions.DTOs.Administration;
using System.Collections.Generic;
using System.Linq;

namespace FS.TimeTracking.Application.Tests.Extensions
{
    public static class PermissionExtensions
    {
        public static List<PermissionDto> ChangePermission(this List<PermissionDto> permissions, string permissionName, string scopeName)
        {
            var permission = permissions.First(p => p.Name == permissionName);
            permission.Scope = scopeName;
            return permissions;
        }
    }
}
