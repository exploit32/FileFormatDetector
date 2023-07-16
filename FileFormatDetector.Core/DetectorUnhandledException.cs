using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileFormatDetector.Core
{
    /// <summary>
    /// Detector exception callback argument
    /// </summary>
    public class DetectorUnhandledException
    {
        /// <summary>
        /// File that caused exception
        /// </summary>
        public string File { get; }

        /// <summary>
        /// Detector that parsed file
        /// </summary>
        public object Detector { get; }

        /// <summary>
        /// Exception
        /// </summary>
        public Exception Exception { get; }

        public DetectorUnhandledException(string file, object detector, Exception exception)
        {
            File = file;
            Detector = detector;
            Exception = exception;
        }
    }
}
