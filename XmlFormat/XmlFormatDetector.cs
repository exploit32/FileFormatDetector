using FormatApi;
using System.Text;

namespace XmlFormat
{
    public class XmlFormatDetector : ITextBasedFormatDetector, IConfigurableDetector
    {
        static XmlFormatDetector()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        const string ValidateFullXmlParameterName = "validate-full-xml";

        private static ParameterDescription[] Parameters = new ParameterDescription[] { new ParameterDescription(ValidateFullXmlParameterName, "Falidate full xml document", true) };

        public bool ValidateFullXml { get; set; } = false;

        public IEnumerable<ParameterDescription> GetParameters() => Parameters;

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

        public void SetParameter(string key, string value)
        {
            if (key == ValidateFullXmlParameterName)
            {
                ValidateFullXml = true;
            }
        }
    }
}