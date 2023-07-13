using FileFormatDetector.Core;

namespace FileFormatDetector.Console
{
    internal class Program
    {
        private const string PluginsDirectory = "Plugins";

        static async Task<int> Main(string[] args)
        {
            var commandLineParser = new CommandLineParser(args);

            AppConfiguration configuration;

            try
            {
                if (commandLineParser.HelpRequested())
                {
                    commandLineParser.PrintHelp();
                    return (int)ExitCodes.OK;
                }

                configuration = commandLineParser.Parse();
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

            FormatPluginsLoader loader = new FormatPluginsLoader(PluginsDirectory);

            loader.LoadPlugins();

            if (!loader.AnyPluginsLoaded)
            {
                System.Console.WriteLine($"No format pluging were found. Build solution to put them to {PluginsDirectory} directory");
                return (int)ExitCodes.NoPlugins;
            }

            FormatDetector detector = new FormatDetector(configuration.DetectorConfiguration, loader.BinaryFormatDetectors.ToArray(), loader.TextFormatDetectors.ToArray(), loader.TextBasedFormatDetectors.ToArray());

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