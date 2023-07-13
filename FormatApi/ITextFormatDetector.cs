using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormatApi
{
    public interface ITextFormatDetector
    {
        Task<TextFormatSummary?> ReadFormat(Stream stream, CancellationToken cancellationToken);
    }
}
