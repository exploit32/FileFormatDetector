using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileFormatDetector.Core
{
    /// <summary>
    /// Exception occured during file recognition
    /// </summary>
    public class FileRecognitionException: Exception
    {
        /// <summary>
        /// Detector that threw an exception
        /// </summary>
        public object Detector { get; private set; }

        /// <summary>
        /// Type of fault
        /// </summary>
        public FaultType FaultType { get; private set; }

        public FileRecognitionException(object detector, FaultType faultType, string? message, Exception? innerException) : base(message, innerException)
        {
            Detector = detector;
            FaultType = faultType;
        }        
    }
}
