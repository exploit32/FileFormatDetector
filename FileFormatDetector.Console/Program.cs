using FileFormatDetector.Core;
using Microsoft.Extensions.Configuration;


namespace FileFormatDetector.Console
{
    internal class Program
    {
        private const string PluginsDirectory = "Plugins";

        public static IConfiguration Configuration { get; set; }

        static async Task<int> Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
            .AddCommandLine(args);

            Configuration = builder.Build();

            AppConfiguration detectorConfiguration = new AppConfiguration();

            Configuration.Bind(detectorConfiguration);

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            System.Console.CancelKeyPress += (sender, args) =>
            {
                System.Console.WriteLine("Cancellation requested");
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

            FormatDetector detector = new FormatDetector(detectorConfiguration, loader.BinaryFormatDetectors.ToArray(), loader.TextFormatDetectors.ToArray(), loader.TextBasedFormatDetectors.ToArray());

            var recognizedFiles = await detector.ScanFiles(cancellationTokenSource.Token);

            FormatPrinter printer = new FormatPrinter();

            printer.PrintRecognizedFiles(recognizedFiles);

            printer.PrintDistinctCount(recognizedFiles);

            return (int)ExitCodes.OK;
        }

        enum ExitCodes
        {
            OK = 0,
            NoPlugins = 1,
        }
    }
}