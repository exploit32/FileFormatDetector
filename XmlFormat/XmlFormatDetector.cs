using FormatApi;
using System.Text;

namespace XmlFormat
{
    public class XmlFormatDetector : ITextBasedFormatDetector
    {
        public async Task<FormatSummary?> ReadFormat(Stream stream, TextFormatSummary textFormatSummary, long? maxBytesToRead)
        {
            if (textFormatSummary.CodePage == 0)
                throw new FormatException($"Cannot read encoding {textFormatSummary.EncodingName}");

            Encoding encoding;

            try
            {
                encoding = Encoding.GetEncoding(textFormatSummary.CodePage);
            }
            catch (NotSupportedException ex)
            {
                throw new FormatException(ex.Message, ex);
            }

            XmlValidator validator = new XmlValidator();

            bool xmlValid = await validator.ReadFile(stream, encoding, maxBytesToRead);

            if (xmlValid)
            {
                return new XmlFormatSummary();
            }

            return null;
        }
    }
}