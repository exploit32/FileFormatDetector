using FileFormatDetector.Core;

namespace FileFormatDetector.Console
{
    internal class Program
    {
        private const string PluginsDirectory = "Plugins";

        static async Task<int> Main(string[] args)
        {
            FormatPluginsLoader loader = new FormatPluginsLoader(PluginsDirectory);

            loader.LoadPlugins();

            if (!loader.AnyPluginsLoaded)
            {
                System.Console.WriteLine($"No format pluging were found. Build solution to put them into {PluginsDirectory} directory");
                return (int)ExitCodes.NoPlugins;
            }
            else
            {
                if (loader.GeneralFormatDetectors.Any())
                {
                    System.Console.WriteLine("Loaded general format detectors:");
                    foreach (var formatDetector in loader.GeneralFormatDetectors)
                        System.Console.WriteLine($" - {formatDetector.Description}");
                }

                if (loader.TextBasedFormatDetectors.Any())
                {
                    System.Console.WriteLine("Loaded text-based format detectors:");
                    foreach (var formatDetector in loader.TextBasedFormatDetectors)
                        System.Console.WriteLine($" - {formatDetector.Description}");
                }

            }

            AppConfiguration configuration = new AppConfiguration();

            var commandLineParser = new CommandLineParser();

            commandLineParser.Configure(configuration);
            commandLineParser.Configure(loader.GeneralFormatDetectors);
            commandLineParser.Configure(loader.TextBasedFormatDetectors);

            try
            {
                if (commandLineParser.HelpRequested(args))
                {
                    commandLineParser.PrintHelp();
                    return (int)ExitCodes.OK;
                }

                commandLineParser.Parse(args);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"Error parsing command line arguments: {ex.Message}");
                return (int)ExitCodes.InvalidArgs;
            }

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            System.Console.CancelKeyPress += (sender, args) =>
            {
                cancellationTokenSource.Cancel();
                args.Cancel = true;
            };

            var detectorConfiguration = new FormatDetectorConfiguration()
            {
                Recursive = configuration.Recursive,
                Threads = configuration.Threads,
            };

            FormatDetector detector = new FormatDetector(detectorConfiguration,
                                                         loader.GeneralFormatDetectors.ToArray(),
                                                         loader.TextBasedFormatDetectors.ToArray());

            System.Console.WriteLine("Scanning files and directories...");

            var recognizedFiles = await detector.ScanFiles(configuration.Paths.ToArray(), cancellationTokenSource.Token);

            FormatPrinter printer = new FormatPrinter();

            if (configuration.Verbose)
                printer.PrintRecognizedFiles(recognizedFiles);
            
            printer.PrintDistinctCount(recognizedFiles);

            return (int)ExitCodes.OK;
        }

        enum ExitCodes
        {
            OK = 0,
            InvalidArgs = 1,
            NoPlugins = 2,
        }
    }
}