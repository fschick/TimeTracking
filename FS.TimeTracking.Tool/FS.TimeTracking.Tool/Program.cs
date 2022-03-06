using FS.TimeTracking.Tool.Interfaces.Import;
using FS.TimeTracking.Tool.Startup;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace FS.TimeTracking.Tool;

public class Program
{
    public static async Task Main(string[] args)
    {
        var options = new CommandLineOptions(args);
        if (!options.Parsed)
            return;

        await using var serviceProvider = new ServiceCollection()
            .RegisterApplicationServices(options)
            .BuildServiceProvider();

        if (options.ImportKimaiV1)
        {
            await serviceProvider
                .GetRequiredService<IKimaiV1ImportService>()
                .Import();
        }

        if (options.ImportTimeTracking)
        {
            await serviceProvider
                .GetRequiredService<ITimeTrackingImportService>()
                .Import();
        }
    }
}