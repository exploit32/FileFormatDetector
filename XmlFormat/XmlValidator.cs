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

                        if (validationMethod == ValidationMethod.UntilFirstNode && xmlReader.NodeType != XmlNodeType.Whitespace)
                            break;

                        cancellationToken.ThrowIfCancellationRequested();
                    }
                }
                catch (Exception)
                {
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

    class XmlValidationResult
    {
        public bool Valid { get; init; }

        public string? XmlDeclarationEncoding { get; init; }
    }
}
