namespace FileFormatDetector.Core
{
    /// <summary>
    /// Types of fault during file detection
    /// </summary>
    public enum FaultType
    {
        /// <summary>
        /// File format issues
        /// </summary>
        FileFormatError,

        /// <summary>
        /// Detector internal error
        /// </summary>
        DetectorInternalError
    }
}
