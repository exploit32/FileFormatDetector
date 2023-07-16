using FormatApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileFormatDetector.Core
{
    /// <summary>
    /// Class that represents recognition result of a file
    /// </summary>
    public class FileRecognitionResult
    {
        /// <summary>
        /// File path
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// File format
        /// </summary>
        public FormatSummary? FormatSummary { get; init; }

        /// <summary>
        /// Thrown exception
        /// </summary>
        public Exception? Exception { get; init; }

        /// <summary>
        /// Indicates that file was recognized sucessfully
        /// </summary>
        public bool IsRecognized => FormatSummary != null && Exception == null;

        /// <summary>
        /// Indicates that file format wasn't recognised
        /// </summary>
        public bool IsUnknown => FormatSummary == null && Exception == null;

        /// <summary>
        /// Indicates that detector threw an exception
        /// </summary>
        public bool IsFaulted => Exception != null;

        /// <summary>
        /// Indicates that it is a text file
        /// </summary>
        public bool IsTextFile => FormatSummary != null && FormatSummary is TextFormatSummary;

        /// <summary>
        /// Constructs FileRecognitionResult in unknown state
        /// </summary>
        /// <param name="path">File path</param>
        /// <returns>FileRecognitionResult</returns>
        internal static FileRecognitionResult Unknown(string path) => new FileRecognitionResult(path);

        /// <summary>
        /// Constructs FileRecognitionResult in faulted state
        /// </summary>
        /// <param name="path">File path</param>
        /// <param name="ex">Exception</param>
        /// <returns>FileRecognitionResult</returns>
        internal static FileRecognitionResult Faulted(string path, Exception ex) => new FileRecognitionResult(path) { Exception = ex };

        /// <summary>
        /// Constructs FileRecognitionResult in recognized state
        /// </summary>
        /// <param name="path">FilePath</param>
        /// <param name="format">Detected format</param>
        /// <returns>FileRecognitionResult</returns>
        internal static FileRecognitionResult Recognized(string path, FormatSummary format) => new FileRecognitionResult(path) { FormatSummary = format };

        private FileRecognitionResult(string path)
        {
            Path = path;
        }
    }
}
