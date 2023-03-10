using AutoMapper;
using FS.TimeTracking.Abstractions.Constants;
using FS.TimeTracking.Abstractions.DTOs.Administration;
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
        var permissions = DefaultPermissions.NoPermissions;
        foreach (var permission in permissions)
        {
            var viewRoleExists = source.Any(name => name == $"{permission.Name}-{PermissionScope.VIEW}");
            var manageRoleExists = source.Any(name => name == $"{permission.Name}-{PermissionScope.MANAGE}");
            if (manageRoleExists)
                permission.Scope = PermissionScope.MANAGE;
            else if (viewRoleExists)
                permission.Scope = PermissionScope.VIEW;
            else
                permission.Scope = PermissionScope.NONE;
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
                PermissionScope.NONE => Array.Empty<string>(),
                PermissionScope.VIEW => new[] { $"{permission.Name}-{PermissionScope.VIEW}" },
                PermissionScope.MANAGE => new[] { $"{permission.Name}-{PermissionScope.VIEW}", $"{permission.Name}-{PermissionScope.MANAGE}" },
                _ => throw new ArgumentOutOfRangeException(nameof(permission))
            })
            .ToList();
}