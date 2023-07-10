using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileFormatDetector.Core
{
    public class FormatDetectorConfiguration
    {
        /// <summary>
        /// List of files and directories to scan
        /// </summary>
        public string[] Paths { get; set; } = Array.Empty<string>();

        /// <summary>
        /// Number of parallel scanning threads
        /// </summary>
        public int? Threads { get; set; }

        /// <summary>
        /// Scan directories recursively
        /// </summary>
        public bool Recursive { get; set; } = true;

        /// <summary>
        /// Number of bytes to scan for probabilistic format detection. Null means no boundary.
        /// </summary>
        public long? FileScanSizeLimit { get; set; }
    }
}
