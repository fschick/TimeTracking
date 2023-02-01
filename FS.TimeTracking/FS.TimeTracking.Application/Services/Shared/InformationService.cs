using AutoMapper;
using FS.TimeTracking.Abstractions.DTOs.Configuration;
using FS.TimeTracking.Abstractions.DTOs.Shared;
using FS.TimeTracking.Core.Extensions;
using FS.TimeTracking.Core.Interfaces.Application.Services.Shared;
using FS.TimeTracking.Core.Models.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Application.Services.Shared;

/// <inheritdoc />
public class InformationService : IInformationApiService
{
    private readonly IMapper _mapper;
    private readonly TimeTrackingConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="InformationService"/> class.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <param name="mapper">The mapper.</param>
    public InformationService(IOptions<TimeTrackingConfiguration> configuration, IMapper mapper)
    {
        _mapper = mapper;
        _configuration = configuration.Value;
    }

    /// <inheritdoc />
    public Task<ProductInformationDto> GetProductInformation(CancellationToken cancellationToken = default)
        => Task.FromResult(new ProductInformationDto
        {
            Name = GetProductName(cancellationToken).GetAwaiter().GetResult(),
            Version = GetProductVersion(cancellationToken).GetAwaiter().GetResult(),
            Copyright = GetProductCopyright(cancellationToken).GetAwaiter().GetResult(),
        });

    /// <inheritdoc />
    public Task<string> GetProductName(CancellationToken cancellationToken = default)
        => Task.FromResult(AssemblyExtensions.GetProgramProduct());

    /// <inheritdoc />
    public Task<string> GetProductVersion(CancellationToken cancellationToken = default)
        => Task.FromResult(AssemblyExtensions.GetProgramProductVersion());

    /// <inheritdoc />
    public Task<string> GetProductCopyright(CancellationToken cancellationToken = default)
        => Task.FromResult(AssemblyExtensions.GetProgramCopyright());

    /// <inheritdoc />
    public Task<ClientConfigurationDto> GetClientConfiguration(CancellationToken cancellationToken = default)
    {
        var clientConfiguration = _mapper.Map<ClientConfigurationDto>(_configuration);
        return Task.FromResult(clientConfiguration);
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
}