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
        public FormatDetectorConfiguration DetectorConfiguration { get; private set; } = new FormatDetectorConfiguration();

        public bool Verbose { get; set; }
    }
}
