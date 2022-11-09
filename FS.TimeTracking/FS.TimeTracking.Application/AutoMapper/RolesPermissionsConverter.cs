using AutoMapper;
using FS.TimeTracking.Abstractions.Constants;
using FS.TimeTracking.Abstractions.DTOs.Administration;
using FS.TimeTracking.Abstractions.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace FS.TimeTracking.Application.AutoMapper;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Local")]
internal class RolesToPermissionsConverter : ITypeConverter<List<string>, List<PermissionDto>>
{
    public List<PermissionDto> Convert(List<string> source, List<PermissionDto> destination, ResolutionContext context)
    {
        var permissions = DefaultPermissions.Value;
        foreach (var permission in permissions)
        {
            var viewRoleExists = source.Any(name => name == $"{permission.Name}-{ScopeNames.VIEW}");
            var manageRoleExists = source.Any(name => name == $"{permission.Name}-{ScopeNames.MANAGE}");
            if (manageRoleExists)
                permission.Scope = PermissionScope.Manage;
            else if (viewRoleExists)
                permission.Scope = PermissionScope.View;
            else
                permission.Scope = PermissionScope.None;
        }

        return permissions;
    }
}

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Local")]
internal class RolesFromPermissionsConverter : ITypeConverter<List<PermissionDto>, List<string>>
{
    public List<string> Convert(List<PermissionDto> source, List<string> destination, ResolutionContext context)
        => source
            .SelectMany(permission => permission.Scope switch
            {
                PermissionScope.None => Array.Empty<string>(),
                PermissionScope.View => new[] { $"{permission.Name}-{ScopeNames.VIEW}" },
                PermissionScope.Manage => new[] { $"{permission.Name}-{ScopeNames.VIEW}", $"{permission.Name}-{ScopeNames.MANAGE}" },
                _ => throw new ArgumentOutOfRangeException(nameof(permission))
            })
            .ToList();
}