using FormatApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace XmlFormat
{

    internal class XmlValidator
    {
        public async Task<XmlValidationResult> Validate(Stream stream, Encoding encoding, ValidationMethod validationMethod, CancellationToken cancellationToken)
        {
            bool xmlValid = true;
            string xmlDeclarationEncoding = string.Empty;

            XmlReaderSettings settings = new XmlReaderSettings()
            {
                ValidationFlags = System.Xml.Schema.XmlSchemaValidationFlags.None,
                ValidationType = ValidationType.None,
                DtdProcessing = DtdProcessing.Ignore,
                CloseInput = false,
                Async = true,
            };

            bool nonWhitespaceElementFound = false;

            using (StreamReader reader = new StreamReader(stream, encoding, leaveOpen: true))
            using (XmlReader xmlReader = XmlReader.Create(reader, settings))
            {
                try
                {
                    while (await xmlReader.ReadAsync())
                    {
                        if (xmlReader.NodeType == XmlNodeType.XmlDeclaration)
                        {
                            xmlDeclarationEncoding = xmlReader.GetAttribute("encoding") ?? String.Empty;
                        }

                        nonWhitespaceElementFound |= xmlReader.NodeType != XmlNodeType.Whitespace;

                        if (validationMethod == ValidationMethod.UntilFirstNode && xmlReader.NodeType != XmlNodeType.Whitespace)
                            break;

                        cancellationToken.ThrowIfCancellationRequested();
                    }
                }
                catch (XmlException ex) when (ex.Message.StartsWith("Version number"))
                {
                    throw new NotSupportedException(ex.Message, ex);
                }
                catch (Exception ex)
                {
                    //Exception is thrown if validation method is Full and file has valid xml nodes at the beginning.
                    //This file is treated as malformed xml
                    if (validationMethod == ValidationMethod.Full && nonWhitespaceElementFound)
                        throw new FileFormatException(ex.Message, ex);

                    xmlValid = false;
                }
            }

            return new XmlValidationResult()
            {
                Valid = xmlValid,
                XmlDeclarationEncoding = xmlDeclarationEncoding,
            };
        }
    }
}
