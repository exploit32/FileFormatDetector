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
        /// Number of parallel scanning threads
        /// </summary>
        public int? Threads { get; set; }

        /// <summary>
        /// Scan directories recursively
        /// </summary>
        public bool Recursive { get; set; } = true;
    }
}
