using FormatApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextFilesFormat;
using XmlFormat;

namespace Tests.Xml
{
    public class EncodingAndXmlDetection
    {
        [Fact]
        public async Task Utf8NonBomXmlShouldBeDetected()
        {
            //Arrange
            //Act
            FormatSummary? format = await DetectEncodingAndXmlFormat(Path.Combine("Samples", "Xml", "addresses-utf8-no-header.xml"), null, validateFullXml: true);

            //Assert
            Assert.NotNull(format);
            Assert.IsType<XmlFormatSummary>(format);
            var xmlFormat = format as XmlFormatSummary;
            Assert.NotNull(xmlFormat);
            Assert.Equal(Encoding.UTF8.CodePage, xmlFormat.CodePage);
            Assert.Equal("UTF-8", xmlFormat.EncodingName);
            Assert.True(string.IsNullOrEmpty(xmlFormat.XmlDeclarationEncoding));
            Assert.False(xmlFormat.HasBOM);
        }

        [Fact]
        public async Task Utf8NonBomWithXmlDeclarationShouldBeDetected()
        {
            //Arrange
            //Act
            FormatSummary? format = await DetectEncodingAndXmlFormat(Path.Combine("Samples", "Xml", "car-rental-utf8.xml"), null, validateFullXml: false);

            //Assert
            Assert.NotNull(format);
            Assert.IsType<XmlFormatSummary>(format);
            var xmlFormat = format as XmlFormatSummary;
            Assert.NotNull(xmlFormat);
            Assert.Equal(Encoding.UTF8.CodePage, xmlFormat.CodePage);
            Assert.Equal("UTF-8", xmlFormat.EncodingName);
            Assert.Equal("UTF-8", xmlFormat.XmlDeclarationEncoding);
            Assert.False(xmlFormat.HasBOM);
        }

        [Fact]
        public async Task DetectedEncodingMayDefferFromXmlDeclarationEncoding()
        {
            //Arrange
            //Act
            FormatSummary? format = await DetectEncodingAndXmlFormat(Path.Combine("Samples", "Xml", "Invoice-iso-8859-1.xml"), null, validateFullXml: false);

            //Assert
            Assert.NotNull(format);
            Assert.IsType<XmlFormatSummary>(format);
            var xmlFormat = format as XmlFormatSummary;
            Assert.NotNull(xmlFormat);
            Assert.Equal(Encoding.ASCII.CodePage, xmlFormat.CodePage);
            Assert.Equal("ASCII", xmlFormat.EncodingName);
            Assert.Equal("ISO-8859-1", xmlFormat.XmlDeclarationEncoding);
            Assert.False(xmlFormat.HasBOM);
        }

        [Fact]
        public async Task Utf8WithBomShouldBeDetected()
        {
            //Arrange
            //Act
            FormatSummary? format = await DetectEncodingAndXmlFormat(Path.Combine("Samples", "Xml", "GolfCountryClub-utf8-bom.xml"), null, validateFullXml: false);

            //Assert
            Assert.NotNull(format);
            Assert.IsType<XmlFormatSummary>(format);
            var xmlFormat = format as XmlFormatSummary;
            Assert.NotNull(xmlFormat);
            Assert.Equal(Encoding.UTF8.CodePage, xmlFormat.CodePage);
            Assert.Equal("UTF-8", xmlFormat.EncodingName);
            Assert.Equal("UTF-8", xmlFormat.XmlDeclarationEncoding);
            Assert.True(xmlFormat.HasBOM);
        }

        [Fact]
        public async Task Utf16BeNonBomShouldBeDetected()
        {
            //Arrange
            //Act
            FormatSummary? format = await DetectEncodingAndXmlFormat(Path.Combine("Samples", "Xml", "simple-utf16be.xml"), null, validateFullXml: false);

            //Assert
            Assert.NotNull(format);
            Assert.IsType<XmlFormatSummary>(format);
            var xmlFormat = format as XmlFormatSummary;
            Assert.NotNull(xmlFormat);
            Assert.Equal(Encoding.BigEndianUnicode.CodePage, xmlFormat.CodePage);
            Assert.Equal("UTF-16BE", xmlFormat.EncodingName);
            Assert.True(string.IsNullOrEmpty(xmlFormat.XmlDeclarationEncoding));
            Assert.False(xmlFormat.HasBOM);
        }

        [Fact]
        public async Task Utf16BeWithBomShouldBeDetected()
        {
            //Arrange
            //Act
            FormatSummary? format = await DetectEncodingAndXmlFormat(Path.Combine("Samples", "Xml", "simple-utf16be-bom.xml"), null, validateFullXml: false);

            //Assert
            Assert.NotNull(format);
            Assert.IsType<XmlFormatSummary>(format);
            var xmlFormat = format as XmlFormatSummary;
            Assert.NotNull(xmlFormat);
            Assert.Equal(Encoding.BigEndianUnicode.CodePage, xmlFormat.CodePage);
            Assert.Equal("UTF-16BE", xmlFormat.EncodingName);
            Assert.True(string.IsNullOrEmpty(xmlFormat.XmlDeclarationEncoding));
            Assert.True(xmlFormat.HasBOM);
        }

        [Fact]
        public async Task Utf16LeNonBomShouldBeDetected()
        {
            //Arrange
            //Act
            FormatSummary? format = await DetectEncodingAndXmlFormat(Path.Combine("Samples", "Xml", "simple-utf16le.xml"), null, validateFullXml: false);

            //Assert
            Assert.NotNull(format);
            Assert.IsType<XmlFormatSummary>(format);
            var xmlFormat = format as XmlFormatSummary;
            Assert.NotNull(xmlFormat);
            Assert.Equal(Encoding.Unicode.CodePage, xmlFormat.CodePage);
            Assert.Equal("UTF-16LE", xmlFormat.EncodingName);
            Assert.True(string.IsNullOrEmpty(xmlFormat.XmlDeclarationEncoding));
            Assert.False(xmlFormat.HasBOM);
        }

        [Fact]
        public async Task Utf16LeWithBomShouldBeDetected()
        {
            //Arrange
            //Act
            FormatSummary? format = await DetectEncodingAndXmlFormat(Path.Combine("Samples", "Xml", "simple-utf16le-bom.xml"), null, validateFullXml: false);

            //Assert
            Assert.NotNull(format);
            Assert.IsType<XmlFormatSummary>(format);
            var xmlFormat = format as XmlFormatSummary;
            Assert.NotNull(xmlFormat);
            Assert.Equal(Encoding.Unicode.CodePage, xmlFormat.CodePage);
            Assert.Equal("UTF-16LE", xmlFormat.EncodingName);
            Assert.True(string.IsNullOrEmpty(xmlFormat.XmlDeclarationEncoding));
            Assert.True(xmlFormat.HasBOM);
        }

        private async Task<FormatSummary?> DetectEncodingAndXmlFormat(string path, long? maxBytes = null, bool validateFullXml = true)
        {
            TextFilesDetector textDetector = new TextFilesDetector();
            textDetector.MaxBytesToRead = maxBytes;

            XmlFormatDetector xmlDetector = new XmlFormatDetector();
            xmlDetector.ValidateFullXml = validateFullXml;

            FormatSummary? xmlFormat = null;

            using (var stream = File.OpenRead(path))
            {
                TextFormatSummary? textFormat = await textDetector.ReadFormat(stream, CancellationToken.None);

                Assert.NotNull(textFormat);

                stream.Seek(0, SeekOrigin.Begin);

                xmlFormat = await xmlDetector.ReadFormat(stream, textFormat);
            }

            return xmlFormat;
        }
    }
}
