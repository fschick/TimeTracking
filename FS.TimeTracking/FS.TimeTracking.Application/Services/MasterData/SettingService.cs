using AutoMapper;
using FS.TimeTracking.Shared.DTOs.TimeTracking;
using FS.TimeTracking.Shared.Interfaces.Application.Services.MasterData;
using FS.TimeTracking.Shared.Interfaces.Repository.Services;
using FS.TimeTracking.Shared.Models.MasterData;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FS.TimeTracking.Shared.DTOs.MasterData;

namespace FS.TimeTracking.Application.Services.MasterData;

/// <inheritdoc cref="ISettingService" />
public class SettingService : ISettingService
{
    private readonly IRepository _repository;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="SettingService"/> class.
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <param name="mapper">The mapper.</param>
    public SettingService(IRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    /// <inheritdoc />
    public async Task<SettingDto> Get(CancellationToken cancellationToken = default)
    {
        var settings = await _repository.Get((Setting x) => x, cancellationToken: cancellationToken);
        if (settings.Count == 0)
            return new SettingDto();
        return _mapper.Map<SettingDto>(settings);
    }

    /// <inheritdoc />
    public async Task Update(SettingDto settings)
    {
        var settingsList = _mapper.Map<List<Setting>>(settings);
        foreach (var setting in settingsList)
        {
            if (!await _repository.Exists((Setting x) => x.Key == setting.Key))
                await _repository.Add(setting);
            else
                _repository.Update(setting);
        }

        await _repository.SaveChanges();
    }
}