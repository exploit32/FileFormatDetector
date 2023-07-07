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
        public async Task<bool> ReadFile(Stream stream, Encoding encoding, long? maxBytesToRead)
        {
            bool xmlValid = true;

            XmlReaderSettings settings = new XmlReaderSettings()
            {
                ValidationFlags = System.Xml.Schema.XmlSchemaValidationFlags.None,
                ValidationType = ValidationType.None,
                DtdProcessing = DtdProcessing.Ignore,
                CloseInput = false
            };

            FileStreamOptions options = new FileStreamOptions()
            {

            };

            using (StreamReader reader = new StreamReader(stream, encoding, leaveOpen: true, bufferSize: 4096))
            using (XmlReader xmlReader = XmlReader.Create(reader, settings))
            {
                try
                {
                    while (xmlReader.Read())
                    {
                        //Console.WriteLine(xmlReader.Value);
                    }
                }
                catch (Exception ex)
                {
                    //Console.WriteLine(ex.ToString());
                    xmlValid = false;
                }
            }

            return xmlValid;
        }
    }
}
