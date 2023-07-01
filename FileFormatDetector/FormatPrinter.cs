using FormatApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileFormatDetector
{
    internal class FormatPrinter
    {
        public void PrintRecognizedFiles(IEnumerable<RecognizedFile> files)
        {
            foreach (var file in files)
            {
                Console.WriteLine($"File {file.FileName} recognized as {file.FormatSummary.FormatName}");

                PrintFormatSummary(file.FormatSummary, string.Empty);                
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
                            Console.WriteLine($"{innerPadding}---------------------");

                        PrintFormatSummary(summaries[i], innerPadding);
                    }                    
                }
                else
                {
                    Console.WriteLine($"{padding}{key}: {value}");
                }
            }
        }

        public void PrintDistinctCount(IEnumerable<RecognizedFile> files)
        {
            var distinctFormats = files.GroupBy(f => f.FormatSummary);

            foreach (var formatGroup in distinctFormats)
            {
                var format = formatGroup.Key;
                var count = formatGroup.Count();

                Console.WriteLine("{0} - {1}", count, format.ToString());
            }
        }

        private string GetFormatShortSummary(FormatSummary formatSummary)
        {
            var keys = formatSummary.GetKeys();

            return string.Format("{0}: {1}", formatSummary.FormatName, String.Join(", ", keys.Select(k => formatSummary[k])));
        }
    }
}
