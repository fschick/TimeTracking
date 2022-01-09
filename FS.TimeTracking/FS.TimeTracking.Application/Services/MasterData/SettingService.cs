using AutoMapper;
using FS.TimeTracking.Shared.DTOs.MasterData;
using FS.TimeTracking.Shared.Interfaces.Application.Services.MasterData;
using FS.TimeTracking.Shared.Interfaces.Repository.Services;
using Nito.AsyncEx;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FS.TimeTracking.Shared.Models.Application.MasterData;

namespace FS.TimeTracking.Application.Services.MasterData;

/// <inheritdoc cref="ISettingService" />
public class SettingService : ISettingService
{
    private readonly IRepository _repository;
    private readonly IMapper _mapper;
    private AsyncLazy<SettingDto> _settingsCache;

    /// <summary>
    /// Initializes a new instance of the <see cref="SettingService"/> class.
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <param name="mapper">The mapper.</param>
    public SettingService(IRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;

        _settingsCache = new AsyncLazy<SettingDto>(async () => await LoadSettings());
    }

    /// <inheritdoc />
    public async Task<SettingDto> Get(CancellationToken cancellationToken = default)
        => await _settingsCache;

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
        _settingsCache = new AsyncLazy<SettingDto>(async () => await LoadSettings());
    }

    private async Task<SettingDto> LoadSettings()
    {
        var settings = await _repository.Get((Setting x) => x);
        return settings.Any()
            ? _mapper.Map<SettingDto>(settings)
            : new SettingDto();
    }
}