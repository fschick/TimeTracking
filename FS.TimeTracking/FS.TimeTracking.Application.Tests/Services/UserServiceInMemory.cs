using AutoMapper;
using FS.TimeTracking.Abstractions.DTOs.Administration;
using FS.TimeTracking.Core.Interfaces.Application.Services.Administration;
using FS.TimeTracking.Core.Models.Filter;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Application.Tests.Services;

public class UserServiceInMemory : IUserApiService
{
    private readonly IMapper _mapper;
    private readonly ConcurrentDictionary<Guid, UserDto> _users = new();

    public IIdentity CurrentUser => throw new NotImplementedException();

    public UserServiceInMemory(IMapper mapper)
        => _mapper = mapper;

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

    private static Func<UserDto, bool> CreateUserFilter(TimeSheetFilterSet filters)
        => filters.UserFilter.CreateFilter()?.Compile() ?? (_ => true);
}