using Autofac.Extras.FakeItEasy;
using AutoMapper;
using FakeItEasy;
using FS.TimeTracking.Abstractions.Constants;
using FS.TimeTracking.Abstractions.DTOs.Administration;
using FS.TimeTracking.Application.Services.Shared;
using FS.TimeTracking.Core.Interfaces.Application.Services.Shared;
using FS.TimeTracking.Core.Interfaces.Repository.Services.Database;
using FS.TimeTracking.Core.Models.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace FS.TimeTracking.Application.Tests.Services.FakeServices;

public class FakeAuthorizationService
{
    private readonly Faker _faker;
    private readonly AutoFake _autoFake;

    public FakeAuthorizationService(Faker faker, AutoFake autoFake)
    {
        _faker = faker;
        _autoFake = autoFake;
    }

    public IAuthorizationService Create(UserDto currentUser = null)
    {
        if (_autoFake == null)
            throw new InvalidOperationException("AutoFake instance required to create fake services.");

        var httpContextAccessor = _autoFake.Resolve<IHttpContextAccessor>();
        A.CallTo(() => httpContextAccessor.HttpContext.User).Returns(CreatePrincipal(currentUser));
        var configuration = _autoFake.Resolve<IOptions<TimeTrackingConfiguration>>();
        var repository = _autoFake.Resolve<IDbRepository>();
        return new AuthorizationService(httpContextAccessor, configuration, repository);
    }

    private ClaimsPrincipal CreatePrincipal(UserDto currentUser)
    {
        currentUser ??= new UserDto();
        var mapper = _autoFake.Resolve<IMapper>();
        var userRoles = mapper.Map<List<string>>(currentUser.Permissions);
        var allRoles = (userRoles ?? RoleNames.All).Select(name => new Claim(ClaimTypes.Role, name));
        var claims = new List<Claim>(allRoles)
        {
            new (ClaimTypes.NameIdentifier, currentUser.Id.ToString()),
            new (ClaimTypes.Name, currentUser.Username ?? string.Empty),
        };
        var identity = new ClaimsIdentity(claims);
        return new ClaimsPrincipal(identity);
    }
}