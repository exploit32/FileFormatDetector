using FileFormatDetector.Core;
using Microsoft.Extensions.Configuration;


namespace FileFormatDetector.Console
{
    internal class Program
    {
        public static IConfiguration Configuration { get; set; }

        static async Task Main(string[] args)
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

            FormatPluginsLoader loader = new FormatPluginsLoader("Plugins");

            loader.LoadPlugins();

            //IBinaryFormatDetector[] binaryFormats = new IBinaryFormatDetector[] { new PEFormatDetector(), new ElfFormatDetector(), new MachOFormatDetector() };

            //ITextFormatDetector[] textFormats = new ITextFormatDetector[] { new TextFilesDetector() };

            //ITextBasedFormatDetector[] textBasedFormatDetectors = new ITextBasedFormatDetector[] { new XmlFormatDetector() };

            FormatDetector detector = new FormatDetector(detectorConfiguration, loader.BinaryFormatDetectors.ToArray(), loader.TextFormatDetectors.ToArray(), loader.TextBasedFormatDetectors.ToArray());

            var recognizedFiles = await detector.ScanFiles(cancellationTokenSource.Token);

            FormatPrinter printer = new FormatPrinter();

            printer.PrintRecognizedFiles(recognizedFiles);

            printer.PrintDistinctCount(recognizedFiles);
        }
    }
}