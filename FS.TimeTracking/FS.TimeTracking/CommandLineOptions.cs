using Mono.Options;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace FS.TimeTracking
{
    /// <summary>
    /// Command line options
    /// </summary>
    public class CommandLineOptions
    {
        /// <summary>
        /// Gets or sets the open API spec file path to generate.
        /// </summary>
        public string OpenApiSpecFile { get; set; }

        /// <summary>
        /// Gets a value indicating whether an open API spec file should generated.
        /// </summary>
        public bool GenerateOpenApiSpecFile => !string.IsNullOrWhiteSpace(OpenApiSpecFile);

        /// <summary>
        /// Gets or sets the validation spec file path to generate.
        /// </summary>
        public string ValidationSpecFile { get; set; }

        /// <summary>
        /// Gets a value indicating whether an validation spec file should generated.
        /// </summary>
        public bool GenerateValidationSpecFile => !string.IsNullOrWhiteSpace(ValidationSpecFile);

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="CommandLineOptions"/> is parsed successfully.
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
}
