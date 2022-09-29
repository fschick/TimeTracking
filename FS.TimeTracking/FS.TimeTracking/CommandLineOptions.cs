using Mono.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace FS.TimeTracking;

/// <summary>
/// Command line options
/// </summary>
[ExcludeFromCodeCoverage]
public class CommandLineOptions
{
    /// <summary>
    /// File path to generated OpenAPI spec file
    /// </summary>
    public string OpenApiSpecFile { get; set; }

    /// <summary>
    /// Generates an OpenAPI spec file
    /// </summary>
    public bool GenerateOpenApiSpecFile => !string.IsNullOrWhiteSpace(OpenApiSpecFile);

    /// <summary>
    /// File path to generated validation spec file
    /// </summary>
    public string ValidationSpecFile { get; set; }

    /// <summary>
    /// Generates an validation spec file
    /// </summary>
    public bool GenerateValidationSpecFile => !string.IsNullOrWhiteSpace(ValidationSpecFile);

    /// <summary>
    /// Indicates whether the command line options were parsed successfully.
    /// </summary>
    public bool Parsed { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandLineOptions"/> class.
    /// </summary>
    /// <param name="args">The command line arguments.</param>
    public CommandLineOptions(IEnumerable<string> args)
    {
        var showHelp = false;

        var optionSet = new OptionSet {
            { "o|generate-openapi=", "Generate OpenAPI spec", x => OpenApiSpecFile = x },
            { "v|generate-validation=", "Generate validation spec", x => ValidationSpecFile = x },
            { "h|help", "Show this message and exit", x => showHelp = x != null },
        };

        try
        {
            optionSet.Parse(args);
            if (showHelp)
                ShowHelp(optionSet);
            else
                Parsed = true;
        }
        catch (OptionException ex)
        {
            // show some app description message
            Console.WriteLine($"Usage: {Assembly.GetExecutingAssembly().GetName()} [OPTIONS]+");
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine();

            // output the options
            Console.WriteLine("Options:");
            ShowHelp(optionSet);
        }
    }

    private static void ShowHelp(OptionSet optionSet)
        => optionSet.WriteOptionDescriptions(Console.Out);
}