﻿using FormatApi;
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

        public string Description => "Xml files detector. Supports xml version 1.0.";

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
                return new XmlFormatSummary()
                {
                    CodePage = textFormatSummary.CodePage,
                    EncodingFullName = textFormatSummary.EncodingFullName,
                    EncodingName = textFormatSummary.EncodingName,
                    HasBOM = textFormatSummary.HasBOM,
                    XmlDeclarationEncoding = validationResult.XmlDeclarationEncoding ?? string.Empty,
                };
            }

            return null;
        }
    }
}