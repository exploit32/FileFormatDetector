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

            AppConfiguration configuration = new AppConfiguration();

            var commandLineParser = new CommandLineParser();

            commandLineParser.Configure(configuration);
            commandLineParser.Configure(loader.BinaryFormatDetectors);
            commandLineParser.Configure(loader.TextFormatDetectors);
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
                                                         loader.BinaryFormatDetectors.ToArray(),
                                                         loader.TextFormatDetectors.ToArray(),
                                                         loader.TextBasedFormatDetectors.ToArray());

            var recognizedFiles = await detector.ScanFiles(configuration.Paths, cancellationTokenSource.Token);

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