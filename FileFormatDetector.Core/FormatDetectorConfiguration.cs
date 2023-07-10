using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileFormatDetector.Core
{
    public class FormatDetectorConfiguration
    {
        public string[] Paths { get; set; } = Array.Empty<string>();

        public int? Threads { get; set; }

        public bool Recursive { get; set; } = true;
    }
}
