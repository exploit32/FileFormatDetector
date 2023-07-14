using FormatApi;
using System.Text;

namespace XmlFormat
{
    public class XmlFormatDetector : ITextBasedFormatDetector
    {
        static XmlFormatDetector()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }


        [Parameter("validate-full-xml", "Validate full xml document.\nBy default the file is read up to the first\ncorrect non-whitespace element")]
        public bool ValidateFullXml { get; set; } = false;

        public async Task<FormatSummary?> ReadFormat(Stream stream, TextFormatSummary textFormatSummary)
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

            var validationResult = await validator.Validate(stream, encoding, ValidateFullXml ? ValidationMethod.Full : ValidationMethod.UntilFirstNode);

            if (validationResult.Valid)
            {
                return new XmlFormatSummary()
                {
                    CodePage = textFormatSummary.CodePage,
                    EncodingFullName = textFormatSummary.EncodingFullName,
                    EncodingName = textFormatSummary.EncodingName,
                    HasBOM = textFormatSummary.HasBOM,
                    XmlDeclarationEncoding = validationResult.XmlDeclarationEncoding,
                };
            }

            return null;
        }
    }
}