using FormatApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XmlFormat;

namespace Tests.Xml
{
    public class XmlFormatTests
    {
        [Fact]
        public async Task SimpleXmlShouldBeParsed()
        {
            //Arrange
            string xml = 
@"<note>
  <date>2015-09-01</date>
  <hour>08:30</hour>
  <to>Tove</to>
  <from>Jani</from>
  <body>Don't forget me this weekend!</body>
</note>
";
            Encoding encoding = Encoding.UTF8;

            //Act
            FormatSummary? format = await EncodeAndDetect(xml, encoding);

            //Assert
            Assert.NotNull(format);
            Assert.IsType<XmlFormatSummary>(format);
            var xmlFormat = format as XmlFormatSummary;
            Assert.NotNull(xmlFormat);
            Assert.Equal(encoding.CodePage, xmlFormat.CodePage);
            Assert.Equal(encoding.EncodingName, xmlFormat.EncodingName);
            Assert.True(string.IsNullOrEmpty(xmlFormat.XmlDeclarationEncoding));
        }

        [Fact]
        public async Task LeadingWhitespacesShouldBeIgnored()
        {
            //Arrange
            string xml =
@"

     <note>
  <date>2015-09-01</date>
  <hour>08:30</hour>
  <to>Tove</to>
  <from>Jani</from>
  <body>Don't forget me this weekend!</body>
</note>
";
            Encoding encoding = Encoding.UTF8;

            //Act
            FormatSummary? format = await EncodeAndDetect(xml, encoding);

            //Assert
            Assert.NotNull(format);
            Assert.IsType<XmlFormatSummary>(format);
            var xmlFormat = format as XmlFormatSummary;
            Assert.NotNull(xmlFormat);
            Assert.Equal(encoding.CodePage, xmlFormat.CodePage);
            Assert.Equal(encoding.EncodingName, xmlFormat.EncodingName);
            Assert.True(string.IsNullOrEmpty(xmlFormat.XmlDeclarationEncoding));
        }

        [Fact]
        public async Task XmlDeclarationWithEncodingShouldBeDetected()
        {
            //Arrange
            string xml =
@"<?xml version=""1.0"" encoding=""UTF-8""?>
<note>
  <date>2015-09-01</date>
  <hour>08:30</hour>
  <to>Tove</to>
  <from>Jani</from>
  <body>Don't forget me this weekend!</body>
</note>
";
            Encoding encoding = Encoding.UTF8;

            //Act
            FormatSummary? format = await EncodeAndDetect(xml, encoding);

            //Assert
            Assert.NotNull(format);
            Assert.IsType<XmlFormatSummary>(format);
            var xmlFormat = format as XmlFormatSummary;
            Assert.NotNull(xmlFormat);
            Assert.Equal(encoding.CodePage, xmlFormat.CodePage);
            Assert.Equal(encoding.EncodingName, xmlFormat.EncodingName);
            Assert.Equal("UTF-8", xmlFormat.XmlDeclarationEncoding);
        }

        [Fact]
        public async Task XmlDeclarationWithoutEncodingShouldBeAccepted()
        {
            //Arrange
            string xml =
@"<?xml version=""1.0"" ?>
<note>
  <date>2015-09-01</date>
  <hour>08:30</hour>
  <to>Tove</to>
  <from>Jani</from>
  <body>Don't forget me this weekend!</body>
</note>
";
            Encoding encoding = Encoding.UTF8;

            //Act
            FormatSummary? format = await EncodeAndDetect(xml, encoding);

            //Assert
            Assert.NotNull(format);
            Assert.IsType<XmlFormatSummary>(format);
            var xmlFormat = format as XmlFormatSummary;
            Assert.NotNull(xmlFormat);
            Assert.Equal(encoding.CodePage, xmlFormat.CodePage);
            Assert.Equal(encoding.EncodingName, xmlFormat.EncodingName);
            Assert.True(string.IsNullOrEmpty(xmlFormat.XmlDeclarationEncoding));
        }

        [Fact]
        public async Task WhitespacesBeforeXmlDeclarationShouldNotBeAllowed()
        {
            //Arrange
            string xml =
@" <?xml version=""1.0"" encoding=""UTF-8""?>
<note>
  <date>2015-09-01</date>
  <hour>08:30</hour>
  <to>Tove</to>
  <from>Jani</from>
  <body>Don't forget me this weekend!</body>
</note>
";
            Encoding encoding = Encoding.UTF8;

            //Act
            FormatSummary? format = await EncodeAndDetect(xml, encoding);

            //Assert
            Assert.Null(format);
        }

        [Fact]
        public async Task ValidateFirstTagModeShouldIgnoreOtherContent()
        {
            //Arrange
            string xml =
@"<?xml version=""1.0""?>
<no..
";
            Encoding encoding = Encoding.UTF8;

            //Act
            FormatSummary? format = await EncodeAndDetect(xml, encoding, validateFullXml: false);

            //Assert
            Assert.NotNull(format);
            Assert.IsType<XmlFormatSummary>(format);
            var xmlFormat = format as XmlFormatSummary;
            Assert.NotNull(xmlFormat);
            Assert.Equal(encoding.CodePage, xmlFormat.CodePage);
            Assert.Equal(encoding.EncodingName, xmlFormat.EncodingName);
            Assert.True(string.IsNullOrEmpty(xmlFormat.XmlDeclarationEncoding));
        }

        [Fact]
        public async Task ValidateFirstTagModeShouldSkipLeadingWhitespaces()
        {
            //Arrange
            string xml =
@"

     <note>
";
            Encoding encoding = Encoding.UTF8;

            //Act
            FormatSummary? format = await EncodeAndDetect(xml, encoding, validateFullXml: false);

            //Assert
            Assert.NotNull(format);
            Assert.IsType<XmlFormatSummary>(format);
            var xmlFormat = format as XmlFormatSummary;
            Assert.NotNull(xmlFormat);
            Assert.Equal(encoding.CodePage, xmlFormat.CodePage);
            Assert.Equal(encoding.EncodingName, xmlFormat.EncodingName);
            Assert.True(string.IsNullOrEmpty(xmlFormat.XmlDeclarationEncoding));
        }

        [Fact]
        public async Task NonXmlFileShouldNotBeDetected()
        {
            //Arrange
            string xml =
@"Lorem ipsum dolor sit amet
";
            Encoding encoding = Encoding.UTF8;

            //Act
            FormatSummary? format = await EncodeAndDetect(xml, encoding, validateFullXml: false);

            //Assert
            Assert.Null(format);
        }

        [Fact]
        public async Task XmlVersion11ShouldThrowNotSupportedException()
        {
            //Arrange
            string xml =
@"<?xml version=""1.1"" encoding=""UTF-8""?>
<note>
  <date>2015-09-01</date>
  <hour>08:30</hour>
  <to>Tove</to>
  <from>Jani</from>
  <body>Don't forget me this weekend!</body>
</note>
";
            Encoding encoding = Encoding.UTF8;

            //Act
            //Assert
            await Assert.ThrowsAsync<NotSupportedException>(async () => await EncodeAndDetect(xml, encoding));
        }

        [Fact]
        public async Task FullCheckShouldThrowExceptionIfInvalidNodesWasFoundAfterValid()
        {
            //Arrange
            string xml =
@"<?xml version=""1.0"" encoding=""UTF-8""?>
No more xml
";
            Encoding encoding = Encoding.UTF8;

            //Act
            //Assert
            await Assert.ThrowsAsync<FileFormatException>(async () => await EncodeAndDetect(xml, encoding));
        }

        [Fact]
        public async Task NotSupportedExceptionShouldBeThrownOnUnknownEncoding()
        {
            //Arrange
            string xml =
@"<?xml version=""1.0"" encoding=""UTF-8""?>
<note>
</note>
";

            XmlFormatDetector detector = new XmlFormatDetector();

            using (var stream = new MemoryStream())
            {
                using (var writer = new StreamWriter(stream, Encoding.UTF8, leaveOpen: true))
                {
                    writer.Write(xml);
                    writer.Flush();
                }

                stream.Seek(0, SeekOrigin.Begin);

                //Act
                //Assert
                await Assert.ThrowsAsync<NotSupportedException>(async () => await detector.ReadFormat(stream, new FormatApi.TextFormatSummary() { CodePage = 1, EncodingName = "Unknown encoding", HasBOM = true }, CancellationToken.None));
            }
        }

        [Fact]
        public async Task IOExceptionsShouldBeRethrown()
        {
            //Arrange
            string xml =
@"<?xml version=""1.0"" encoding=""UTF-8""?>
<note>
</note>
";

            XmlFormatDetector detector = new XmlFormatDetector();

            using (var stream = new MemoryStream())
            {
                using (var writer = new StreamWriter(stream, Encoding.UTF8, leaveOpen: true))
                {
                    writer.Write(xml);
                    writer.Flush();
                }

                stream.Seek(0, SeekOrigin.Begin);

                //Act
                //Assert
                await Assert.ThrowsAsync<NotSupportedException>(async () => await detector.ReadFormat(stream, new FormatApi.TextFormatSummary() { CodePage = 1, EncodingName = "Unknown encoding", HasBOM = true }, CancellationToken.None));
            }
        }


        private async Task<FormatSummary?> EncodeAndDetect(string xml, Encoding encoding, bool validateFullXml = true)
        {
            XmlFormatDetector detector = new XmlFormatDetector
            {
                ValidateFullXml = validateFullXml
            };

            FormatSummary? format = null;

            using (var stream = new MemoryStream())
            {
                using (var writer = new StreamWriter(stream, encoding, leaveOpen: true))
                {
                    writer.Write(xml);
                    writer.Flush();
                }

                stream.Seek(0, SeekOrigin.Begin);

                format = await detector.ReadFormat(stream, new FormatApi.TextFormatSummary() { CodePage = encoding.CodePage, EncodingName = encoding.EncodingName, HasBOM = true }, CancellationToken.None);
            }

            return format;
        }
    }
}
