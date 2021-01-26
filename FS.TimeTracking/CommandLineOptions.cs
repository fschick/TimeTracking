using Mono.Options;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace FS.TimeTracking
{
    public class CommandLineOptions
    {
        public bool RunAsService { get; set; }
        public string OpenApiSpecFile { get; set; }
        public bool GenerateOpenApiSpecFile => !string.IsNullOrWhiteSpace(OpenApiSpecFile);
        public string ValidationSpecFile { get; set; }
        public bool GenerateValidationSpecFile => !string.IsNullOrWhiteSpace(ValidationSpecFile);
        public bool Parsed { get; set; }

        public CommandLineOptions(IEnumerable<string> args)
        {
            var showHelp = false;

            var optionSet = new OptionSet {
                { "s|service", "Run as windows service", x => RunAsService = x != null },
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
