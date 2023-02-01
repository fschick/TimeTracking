using AutoMapper;
using FS.TimeTracking.Abstractions.DTOs.MasterData;
using FS.TimeTracking.Core.Interfaces.Application.Services.Administration;
using FS.TimeTracking.Core.Interfaces.Repository.Services.Database;
using FS.TimeTracking.Core.Models.Application.MasterData;
using Nito.AsyncEx;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Application.Services.Administration;

/// <inheritdoc cref="ISettingApiService" />
public class SettingService : ISettingApiService
{
    private readonly IDbRepository _dbRepository;
    private readonly IMapper _mapper;
    private AsyncLazy<SettingDto> _settingsCache;

    /// <summary>
    /// Initializes a new instance of the <see cref="SettingService"/> class.
    /// </summary>
    /// <param name="dbRepository">The repository.</param>
    /// <param name="mapper">The mapper.</param>
    public SettingService(IDbRepository dbRepository, IMapper mapper)
    {
        _dbRepository = dbRepository;
        _mapper = mapper;

        _settingsCache = new AsyncLazy<SettingDto>(async () => await LoadSettings());
    }

    /// <inheritdoc />
    public async Task<SettingDto> GetSettings(CancellationToken cancellationToken = default)
        => await _settingsCache;

    /// <inheritdoc />
    public async Task UpdateSettings(SettingDto settings)
    {
        var settingsList = _mapper.Map<List<Setting>>(settings);
        foreach (var setting in settingsList)
        {
            if (!await _dbRepository.Exists((Setting x) => x.Key == setting.Key))
                await _dbRepository.Add(setting);
            else
                _dbRepository.Update(setting);
        }

        await _dbRepository.SaveChanges();
        _settingsCache = new AsyncLazy<SettingDto>(async () => await LoadSettings());
    }

    private async Task<SettingDto> LoadSettings()
    {
        var settings = await _dbRepository.Get((Setting x) => x);
        return settings.Any()
            ? _mapper.Map<SettingDto>(settings)
            : new SettingDto();
    }
}