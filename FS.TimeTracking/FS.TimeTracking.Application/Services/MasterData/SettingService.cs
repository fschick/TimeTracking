using AutoMapper;
using FS.TimeTracking.Abstractions.DTOs.Configuration;
using FS.TimeTracking.Abstractions.DTOs.MasterData;
using FS.TimeTracking.Core.Interfaces.Application.Services.MasterData;
using FS.TimeTracking.Core.Interfaces.Repository.Services;
using FS.TimeTracking.Core.Models.Application.MasterData;
using FS.TimeTracking.Core.Models.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Nito.AsyncEx;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Application.Services.MasterData;

/// <inheritdoc cref="ISettingService" />
public class SettingService : ISettingService
{
    private readonly IDbRepository _dbRepository;
    private readonly IMapper _mapper;
    private readonly TimeTrackingConfiguration _configuration;
    private AsyncLazy<SettingDto> _settingsCache;

    /// <summary>
    /// Initializes a new instance of the <see cref="SettingService"/> class.
    /// </summary>
    /// <param name="dbRepository">The repository.</param>
    /// <param name="mapper">The mapper.</param>
    /// <param name="configuration">The configuration.</param>
    public SettingService(IDbRepository dbRepository, IMapper mapper, IOptions<TimeTrackingConfiguration> configuration)
    {
        _dbRepository = dbRepository;
        _mapper = mapper;
        _configuration = configuration.Value;

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

    /// <inheritdoc />
    public async Task<JObject> GetTranslations(string language, CancellationToken cancellationToken = default)
    {
        var translationFolder = Path.Combine(TimeTrackingConfiguration.PathToContentRoot, TimeTrackingConfiguration.TRANSLATION_FOLDER);
        var translationFile = Path.Combine(translationFolder, $"translations.{language}.json");
        if (!File.Exists(translationFile) && language != null)
            translationFile = Path.Combine(translationFolder, $"translations.{language[..2]}.json");
        if (!File.Exists(translationFile))
            translationFile = Path.Combine(translationFolder, "translations.en.json");
        if (!File.Exists(translationFile))
            return new JObject();

        return JObject.Parse(await File.ReadAllTextAsync(translationFile, cancellationToken));
    }

    /// <inheritdoc />
    public Task<ClientConfigurationDto> ClientConfiguration(CancellationToken cancellationToken = default)
    {
        var clientConfiguration = _mapper.Map<ClientConfigurationDto>(_configuration);
        return Task.FromResult(clientConfiguration);
    }

    private async Task<SettingDto> LoadSettings()
    {
        var settings = await _dbRepository.Get((Setting x) => x);
        return settings.Any()
            ? _mapper.Map<SettingDto>(settings)
            : new SettingDto();
    }
}