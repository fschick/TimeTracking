using AutoMapper;
using FakeItEasy;
using FS.TimeTracking.Abstractions.Constants;
using FS.TimeTracking.Abstractions.DTOs.Administration;
using FS.TimeTracking.Application.Services.Shared;
using FS.TimeTracking.Core.Interfaces.Application.Services.Shared;
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
        var mapper = _faker.GetRequiredService<IMapper>();
        A.CallTo(() => httpContextAccessor.HttpContext.User).Returns(CreatePrincipal(currentUser, mapper, "DUMMY"));
        var configuration = _faker.GetRequiredService<IOptions<TimeTrackingConfiguration>>();
        return new AuthorizationService(httpContextAccessor, configuration);
    }

    public static ClaimsPrincipal CreatePrincipal(UserDto currentUser, IMapper mapper, string authenticationScheme)
    {
        currentUser ??= new UserDto();

        var userRoles = mapper.Map<List<string>>(currentUser.Permissions) ?? RoleName.All;
        var userRoleClaims = userRoles.Select(name => new Claim(ClaimTypes.Role, name));
        var restrictToCustomerClaims = currentUser.RestrictToCustomerIds.Select(customerId => new Claim(RestrictToCustomer.ATTRIBUTE, customerId.ToString()));

        var identityClaims = new List<Claim>(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, currentUser.Id.ToString()),
                new Claim(ClaimTypes.Name, currentUser.Username ?? string.Empty),
            })
            .Concat(userRoleClaims)
            .Concat(restrictToCustomerClaims)
            .ToList();

        var identity = new ClaimsIdentity(identityClaims, authenticationScheme);
        return new ClaimsPrincipal(identity);
    }
}