using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormatApi
{
    /// <summary>
    /// Interface for text-based format detectors
    /// </summary>
    public interface ITextBasedFormatDetector
    {
        /// <summary>
        /// Description of format detector
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Detect file format
        /// </summary>
        /// <param name="stream">File stream</param>
        /// <param name="textFormatSummary">Text file encoding information</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Detected format or null</returns>
        Task<FormatSummary?> ReadFormat(Stream stream, TextFormatSummary textFormatSummary, CancellationToken cancellationToken);
    }
}
