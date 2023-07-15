using FileFormatDetector.Core;
using FormatApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileFormatDetector.Console
{
    internal class AppConfiguration
    {
        /// <summary>
        /// List of files and directories to scan
        /// </summary>
        [DefaultParameter]
        public string[] Paths { get; set; } = Array.Empty<string>();

        /// <summary>
        /// Print information about each file individually
        /// </summary>
        [Parameter("verbose", "Print summary about each file individually")]
        public bool Verbose { get; set; }

        /// <summary>
        /// Number of parallel scanning threads
        /// </summary>
        [Parameter("threads", "Number of parallel threads (default is number of CPU cores)")]
        public int? Threads { get; set; }

        /// <summary>
        /// Scan directories recursively
        /// </summary>
        [Parameter("no-recursion", "Scan directories non recursively", isInverted: true)]
        public bool Recursive { get; set; } = true;
    }
}
