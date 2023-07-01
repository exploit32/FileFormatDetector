using ElfFormat;
using FormatApi;
using MachOFormat;
using Microsoft.Extensions.Configuration;
using PEFormat;

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

            IBinaryFormatDetector[] formats = new IBinaryFormatDetector[] { new PEFormatDetector(), new ElfFormatDetector(), new MachOFormatDetector() };

            FormatDetector detector = new FormatDetector(detectorConfiguration, formats);

            var recognizedFiles = await detector.ScanFiles(cancellationTokenSource.Token);

            FormatPrinter printer = new FormatPrinter();

            printer.PrintRecognizedFiles(recognizedFiles);

            printer.PrintDistinctCount(recognizedFiles);
        }
    }
}