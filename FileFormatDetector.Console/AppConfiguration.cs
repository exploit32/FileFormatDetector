using FileFormatDetector.Core;
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
        public string[] Paths { get; set; } = Array.Empty<string>();

        /// <summary>
        /// Print information about each file individually
        /// </summary>
        public bool Verbose { get; set; }

        /// <summary>
        /// Detector settings
        /// </summary>
        public FormatDetectorConfiguration DetectorConfiguration { get; private set; } = new FormatDetectorConfiguration();        
    }
}
