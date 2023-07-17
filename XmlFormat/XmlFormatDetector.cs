using FormatApi;
using System.Text;

namespace XmlFormat
{
    /// <summary>
    /// Detector of Xml format files
    /// </summary>
    public class XmlFormatDetector : ITextBasedFormatDetector
    {
        static XmlFormatDetector()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        /// <summary>
        /// Should detector read and validate entire xml or just read until first non-whitespace node is met
        /// </summary>
        [Parameter("validate-full-xml", "Validate full xml document.\nBy default the file is read up to the first\ncorrect non-whitespace element")]
        public bool ValidateFullXml { get; set; } = false;

        public string Description => "Xml files detector. Supports xml version 1.0.";

        /// <summary>
        /// Read file and detect format
        /// </summary>
        /// <param name="stream">Opened file</param>
        /// <param name="textFormatSummary">Information about file's encoding</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Xml format summary or null</returns>
        /// <exception cref="NotSupportedException">Exception is thworn if file has not supported encoding or xml features</exception>
        public async Task<FormatSummary?> ReadFormat(Stream stream, TextFormatSummary textFormatSummary, CancellationToken cancellationToken)
        {
            if (textFormatSummary.CodePage == 0)
                throw new NotSupportedException($"Cannot read encoding {textFormatSummary.EncodingName}");

            Encoding encoding;

            try
            {
                encoding = Encoding.GetEncoding(textFormatSummary.CodePage);
            }
            catch (Exception ex)
            {
                throw new NotSupportedException($"Error creating encoding: {ex.Message}", ex);
            }

            XmlValidator validator = new XmlValidator();

            var validationResult = await validator.Validate(stream, encoding, ValidateFullXml ? ValidationMethod.Full : ValidationMethod.UntilFirstNode, cancellationToken);

            if (validationResult.Valid)
            {
                return new XmlFormatSummary(
                    xmlDeclarationEncoding: validationResult.XmlDeclarationEncoding ?? string.Empty,
                    baseTextFormat: textFormatSummary
                );
            }

            return null;
        }
    }
}