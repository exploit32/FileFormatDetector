using FormatApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileFormatDetector.Core
{
    public class RecognizedFile
    {
        public string FileName { get; }

        public FormatSummary FormatSummary { get; }

        public RecognizedFile(string fileName, FormatSummary formatSummary)
        {
            FileName = fileName;
            FormatSummary = formatSummary;
        }
    }
}
