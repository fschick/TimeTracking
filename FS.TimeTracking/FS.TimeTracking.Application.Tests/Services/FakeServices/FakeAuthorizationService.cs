using AutoMapper;
using FakeItEasy;
using FS.TimeTracking.Abstractions.Constants;
using FS.TimeTracking.Abstractions.DTOs.Administration;
using FS.TimeTracking.Application.Services.Shared;
using FS.TimeTracking.Core.Interfaces.Application.Services.Shared;
using FS.TimeTracking.Core.Interfaces.Repository.Services.Database;
using FS.TimeTracking.Core.Models.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace FS.TimeTracking.Application.Tests.Services.FakeServices;

public class FakeAuthorizationService
{
    private readonly Faker _faker;

    public FakeAuthorizationService(Faker faker)
        => _faker = faker;

    public IAuthorizationService Create(UserDto currentUser = null)
    {
        var httpContextAccessor = _faker.GetRequiredService<IHttpContextAccessor>();
        A.CallTo(() => httpContextAccessor.HttpContext.User).Returns(CreatePrincipal(currentUser));
        var configuration = _faker.GetRequiredService<IOptions<TimeTrackingConfiguration>>();
        var repository = _faker.GetRequiredService<IDbRepository>();
        return new AuthorizationService(httpContextAccessor, configuration, repository);
    }

    private ClaimsPrincipal CreatePrincipal(UserDto currentUser)
    {
        currentUser ??= new UserDto();
        var mapper = _faker.GetRequiredService<IMapper>();
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