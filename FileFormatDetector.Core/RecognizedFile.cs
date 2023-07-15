using FormatApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileFormatDetector.Core
{
    /// <summary>
    /// Class that represents file which format was recognized
    /// </summary>
    public class RecognizedFile
    {
        /// <summary>
        /// File path
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// File format
        /// </summary>
        public FormatSummary FormatSummary { get; }

        public RecognizedFile(string path, FormatSummary formatSummary)
        {
            Path = path;
            FormatSummary = formatSummary;
        }
    }
}
