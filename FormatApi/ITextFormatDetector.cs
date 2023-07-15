using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormatApi
{
    /// <summary>
    /// Interface of text files detectors
    /// </summary>
    public interface ITextFormatDetector
    {
        Task<TextFormatSummary?> ReadFormat(Stream stream, CancellationToken cancellationToken);
    }
}
