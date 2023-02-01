using FS.TimeTracking.Core.Interfaces.Application.Services.Shared;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Threading.Tasks;

namespace FS.TimeTracking.Api.REST.Startup;

internal static class ValidationDescriptionStartup
{
    public static async Task GenerateValidationSpec(this IHost host, string outFile)
    {
        if (string.IsNullOrWhiteSpace(outFile))
            throw new ArgumentException("No destination file for generated validation document given.");

        var validationDescriptionService = host.Services.GetRequiredService<IValidationDescriptionApiService>();
        var validationSpec = await validationDescriptionService.GetValidationDescriptions();
        var outDirectory = Path.GetDirectoryName(outFile);
        if (outDirectory != null && !Directory.Exists(outDirectory))
            Directory.CreateDirectory(outDirectory);
        await File.WriteAllTextAsync(outFile, validationSpec.ToString(Newtonsoft.Json.Formatting.Indented));
    }
}