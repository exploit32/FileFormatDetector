using FileFormatDetector.Core;
using FormatApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileFormatDetector.Console
{
    /// <summary>
    /// Class that prints recognition results to console
    /// </summary>
    internal class FormatPrinter
    {
        /// <summary>
        /// Group recognized files by unique combination of format and it's parameters and print number of files in each group
        /// </summary>
        /// <param name="files">Collection of recognized files</param>
        public void PrintDistinctCount(IEnumerable<FileRecognitionResult> files)
        {
            var distinctRecognizedFormats = files
                .Where(f => f.IsRecognized)
                .GroupBy(f => f.FormatSummary!)
                .OrderByDescending(f => f.Count());

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
                var recognitionExceptions = faultedFiles.Where(f => f.Exception != null && f.Exception is FileRecognitionException).ToList();

                int formatFaults = recognitionExceptions.Count(e => (e.Exception as  FileRecognitionException)!.FaultType == FaultType.FileFormatError);
                int internalFaults = recognitionExceptions.Count(e => (e.Exception as FileRecognitionException)!.FaultType == FaultType.DetectorInternalError);

                if (formatFaults > 0)
                    System.Console.WriteLine("{0} - File format errors", formatFaults);

                if (internalFaults > 0)
                    System.Console.WriteLine("{0} - Detector internal errors", internalFaults);

                var distinctExceptions = faultedFiles
                    .Where(f => f.Exception != null && !(f.Exception is FileRecognitionException))
                    .GroupBy(f => f.Exception!.GetType())
                    .OrderByDescending(f => f.Count());

                foreach (var exceptionGroup in distinctExceptions)
                {
                    var exception = exceptionGroup.Key.Name;
                    var count = exceptionGroup.Count();

                    System.Console.WriteLine("{0} - Thrown exception: {1}", count, exception.ToString());
                }
            }
        }

        /// <summary>
        /// Print verbose information about all recognized files
        /// </summary>
        /// <param name="files">Collection of files</param>
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
                    if (file.Exception is FileRecognitionException)
                    {
                        var fileRecognitionException = (FileRecognitionException)file.Exception;
                        
                        if (fileRecognitionException.FaultType == FaultType.FileFormatError)
                            System.Console.WriteLine($"Recognition of file {file.Path} threw an exception by detector {fileRecognitionException!.Detector.GetType().Name}: {file.Exception!.Message}");
                        else if (fileRecognitionException.FaultType == FaultType.DetectorInternalError)
                            System.Console.WriteLine($"Recognition of file {file.Path} caused internal error in detector {fileRecognitionException!.Detector.GetType().Name}: {file.Exception!.Message}");
                    }
                    else
                    {
                        System.Console.WriteLine($"Recognition of file {file.Path} threw an exception {file.Exception!.GetType().Name}: {file.Exception!.Message}");
                    }
                }
                else
                {
                    System.Console.WriteLine($"Something went wrong with file {file.Path}. (IsRecognized={file.IsRecognized}, IsUnknown={file.IsUnknown}, IsFaulted={file.IsFaulted})");
                }
            }
        }

        /// <summary>
        /// Print one file format summary
        /// </summary>
        /// <param name="summary">File's formad summary</param>
        /// <param name="padding">Padding. Used for recursive prints</param>
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
    }
}
