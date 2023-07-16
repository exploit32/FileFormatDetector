using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileFormatDetector.Core
{
    public class FileRecognitionException: Exception
    {
        public object Detector { get; private set; }

        public FileRecognitionException(string? message, object detector, Exception? innerException) : base(message, innerException)
        {
            Detector = detector;
        }
    }
}
