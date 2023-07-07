using TextFilesFormat;
using ElfFormat;
using FormatApi;
using MachOFormat;
using Microsoft.Extensions.Configuration;
using PEFormat;
using XmlFormat;

namespace FileFormatDetector
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

            Console.CancelKeyPress += (sender, args) =>
            {
                Console.WriteLine("Cancellation requested");
                cancellationTokenSource.Cancel();
                args.Cancel = true;
            };

            IBinaryFormatDetector[] binaryFormats = new IBinaryFormatDetector[] { new PEFormatDetector(), new ElfFormatDetector(), new MachOFormatDetector() };

            ITextFormatDetector[] textFormats = new ITextFormatDetector[] { new TextFilesDetector() };

            ITextBasedFormatDetector[] textBasedFormatDetectors = new ITextBasedFormatDetector[] { new XmlFormatDetector() };

            FormatDetector detector = new FormatDetector(detectorConfiguration, binaryFormats, textFormats, textBasedFormatDetectors);

            var recognizedFiles = await detector.ScanFiles(cancellationTokenSource.Token);

            FormatPrinter printer = new FormatPrinter();

            printer.PrintRecognizedFiles(recognizedFiles);

            printer.PrintDistinctCount(recognizedFiles);
        }
    }
}