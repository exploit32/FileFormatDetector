using FileFormatDetector.Core;
using FormatApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileFormatDetector.Console
{
    public class FormatPrinter
    {
        public void PrintRecognizedFiles(IEnumerable<FileRecognitionResult> files)
        {
            foreach (var file in files)
            {
                if (file.IsRecognized)
                {
                    System.Console.WriteLine($"File {file.Path} recognized as {file.FormatSummary!.FormatName}");
                    PrintFormatSummary(file.FormatSummary, string.Empty);
                }
                else if (file.IsUnknown)
                {
                    System.Console.WriteLine($"File {file.Path} has unknown format");
                }
                else if (file.IsFaulted)
                {
                    System.Console.WriteLine($"Recognition of file {file.Path} threw an exception {file.Exception!.GetType().Name}: {file.Exception!.Message}");
                }
                else
                {
                    System.Console.WriteLine($"Something went wrong with file {file.Path}. (IsRecognized={file.IsRecognized}, IsUnknown={file.IsUnknown}, IsFaulted={file.IsFaulted})");
                }
            }
        }

        private void PrintFormatSummary(FormatSummary summary, string padding)
        {
            var keys = summary.GetKeys();

            foreach (var key in keys)
            {
                var value = summary[key];

                if (value is FormatSummary[])
                {
                    var summaries = (FormatSummary[])value;

                    string innerPadding = padding + "  ";

                    for (int i = 0; i < summaries.Length; i++)
                    {
                        if (i != 0)
                            System.Console.WriteLine($"{innerPadding}---------------------");

                        PrintFormatSummary(summaries[i], innerPadding);
                    }
                }
                else
                {
                    System.Console.WriteLine($"{padding}{key}: {value}");
                }
            }
        }

        public void PrintDistinctCount(IEnumerable<FileRecognitionResult> files)
        {
            var distinctRecognizedFormats = files.Where(f => f.IsRecognized).GroupBy(f => f.FormatSummary).OrderByDescending(f => f.Count());

            foreach (var formatGroup in distinctRecognizedFormats)
            {
                var format = formatGroup.Key;
                var count = formatGroup.Count();

                System.Console.WriteLine("{0} - {1}", count, format.ToString());
            }

            var unknownFiles = files.Count(f => f.IsUnknown);

            if (unknownFiles > 0)
                System.Console.WriteLine($"{unknownFiles} - Unknown");

            var faultedFiles = files.Where(f => f.IsFaulted).ToList();

            if (faultedFiles.Any())
            {
                var distinctExceptions = faultedFiles.GroupBy(f => f.Exception!.GetType()).OrderByDescending(f => f.Count());

                foreach (var exceptionGroup in distinctExceptions)
                {
                    var format = exceptionGroup.Key.Name;
                    var count = exceptionGroup.Count();

                    System.Console.WriteLine("{0} - thrown exception: {1}", count, format.ToString());
                }
            }
        }
    }
}
