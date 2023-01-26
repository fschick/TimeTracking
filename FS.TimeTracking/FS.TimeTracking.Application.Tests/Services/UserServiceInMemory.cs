using AutoMapper;
using FS.TimeTracking.Abstractions.Constants;
using FS.TimeTracking.Abstractions.DTOs.Administration;
using FS.TimeTracking.Abstractions.Interfaces.DTOs;
using FS.TimeTracking.Core.Interfaces.Application.Services.Administration;
using FS.TimeTracking.Core.Models.Filter;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Application.Tests.Services;

public class UserServiceInMemory : IUserService
{
    private readonly IMapper _mapper;
    private readonly ConcurrentDictionary<Guid, UserDto> _users = new();

    public ClaimsPrincipal CurrentUser { get; }

    public UserServiceInMemory(IMapper mapper, UserDto currentUser)
    {
        _mapper = mapper;
        CurrentUser = CreateCurrentUser(currentUser);
    }

    public Task<UserDto> Get(Guid id, CancellationToken cancellationToken = default)
        => Task.FromResult(_users.Values.FirstOrDefault(x => x.Id == id));

    public Task<List<UserDto>> GetFiltered(TimeSheetFilterSet filters, CancellationToken cancellationToken = default)
    {
        var filter = CreateUserFilter(filters);
        return Task.FromResult(_users.Values.Where(filter).ToList());
    }

    public Task<List<UserGridDto>> GetGridFiltered(TimeSheetFilterSet filters, CancellationToken cancellationToken = default)
    {
        var filter = CreateUserFilter(filters);
        var users = _users.Values.Where(filter);
        return Task.FromResult(_mapper.Map<List<UserGridDto>>(users));
    }

    public Task<UserGridDto> GetGridItem(Guid id, CancellationToken cancellationToken = default)
        => Task.FromResult(_mapper.Map<UserGridDto>(_users.Values.FirstOrDefault(x => x.Id == id)));

    public Task<UserDto> Create(UserDto dto)
    {
        _users.AddOrUpdate(dto.Id, dto, (_, _) => dto);
        return Task.FromResult(dto);
    }

    public Task<UserDto> Update(UserDto dto)
    {
        _users.AddOrUpdate(dto.Id, dto, (_, _) => dto);
        return Task.FromResult(dto);
    }

    public Task<long> Delete(Guid id)
        => Task.FromResult<long>(_users.TryRemove(id, out _) ? 1 : 0);

    public async Task SetUserRelatedProperties<T>(T dto, CancellationToken cancellationToken) where T : class, IUserRelatedGridDto
    {
        var list = new List<T>() { dto };
        await SetUserRelatedProperties(null, list, cancellationToken);
    }

    public async Task SetUserRelatedProperties<T>(TimeSheetFilterSet filters, List<T> dtos, CancellationToken cancellationToken) where T : class, IUserRelatedGridDto
    {
        var filteredUsers = await GetFiltered(filters, cancellationToken);
        dtos = dtos
            .Join(filteredUsers, timeSheet => timeSheet.UserId, user => user.Id, (timeSheet, user) =>
            {
                timeSheet.UserId = user.Id;
                timeSheet.Username = user.Username;
                return timeSheet;
            })
            .ToList();
    }

    private static ClaimsPrincipal CreateCurrentUser(UserDto currentUser)
    {
        currentUser ??= new UserDto();
        var allRoles = RoleNames.All.Select(name => new Claim(ClaimTypes.Role, name));
        var claims = new List<Claim>(allRoles)
        {
            new (ClaimTypes.NameIdentifier, currentUser.Id.ToString()),
            new (ClaimTypes.Name, currentUser.Username ?? string.Empty),
        };
        var identity = new ClaimsIdentity(claims);
        return new ClaimsPrincipal(identity);
    }

    private static Func<UserDto, bool> CreateUserFilter(TimeSheetFilterSet filters)
        => filters.UserFilter.CreateFilter()?.Compile() ?? (_ => true);
}