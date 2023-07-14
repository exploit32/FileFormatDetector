using FormatApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextFilesFormat;

namespace Tests.TextFiles
{
    public class BomTests
    {
        [Fact]
        public async Task Utf8BomShouldBeDetected()
        {
            //Arrange
            string text = "Hello";

            //Act
            FormatSummary? format = await TextTestsHelper.EncodeAndDetectFull(text, Encoding.UTF8, DetectableEncoding.Utf8.BomSignature?.Value);

            //Assert
            Assert.NotNull(format);
            TextTestsHelper.CheckFormat(DetectableEncoding.Utf8, true, format);
        }

        [Fact]
        public async Task Utf8OnlyBomShouldBeDetected()
        {
            //Arrange
            string text = "";

            //Act
            FormatSummary? format = await TextTestsHelper.EncodeAndDetectFull(text, Encoding.UTF8, DetectableEncoding.Utf8.BomSignature?.Value);

            //Assert
            Assert.NotNull(format);
            TextTestsHelper.CheckFormat(DetectableEncoding.Utf8, true, format);
        }

        [Fact]
        public async Task Utf16LEBomShouldBeDetected()
        {
            //Arrange
            string text = "Hello";

            //Act
            FormatSummary? format = await TextTestsHelper.EncodeAndDetectFull(text, Encoding.Unicode, DetectableEncoding.Utf16LE.BomSignature?.Value);

            //Assert
            Assert.NotNull(format);
            TextTestsHelper.CheckFormat(DetectableEncoding.Utf16LE, true, format);
        }

        [Fact]
        public async Task Utf16LEOnlyBomShouldBeDetected()
        {
            //Arrange
            string text = "";

            //Act
            FormatSummary? format = await TextTestsHelper.EncodeAndDetectFull(text, Encoding.Unicode, DetectableEncoding.Utf16LE.BomSignature?.Value);

            //Assert
            Assert.NotNull(format);
            TextTestsHelper.CheckFormat(DetectableEncoding.Utf16LE, true, format);
        }

        [Fact]
        public async Task Utf16BEBomShouldBeDetected()
        {
            //Arrange
            string text = "Hello";

            //Act
            FormatSummary? format = await TextTestsHelper.EncodeAndDetectFull(text, Encoding.BigEndianUnicode, DetectableEncoding.Utf16BE.BomSignature?.Value);

            //Assert
            Assert.NotNull(format);
            TextTestsHelper.CheckFormat(DetectableEncoding.Utf16BE, true, format);
        }

        [Fact]
        public async Task Utf16BEOnlyBomShouldBeDetected()
        {
            //Arrange
            string text = "";

            //Act
            FormatSummary? format = await TextTestsHelper.EncodeAndDetectFull(text, Encoding.BigEndianUnicode, DetectableEncoding.Utf16BE.BomSignature?.Value);

            //Assert
            Assert.NotNull(format);
            TextTestsHelper.CheckFormat(DetectableEncoding.Utf16BE, true, format);
        }

        [Fact]
        public async Task Utf32LEBomShouldBeDetected()
        {
            //Arrange
            string text = "Hello";

            //Act
            FormatSummary? format = await TextTestsHelper.EncodeAndDetectFull(text, Encoding.UTF32, DetectableEncoding.Utf32LE.BomSignature?.Value);

            //Assert
            Assert.NotNull(format);
            TextTestsHelper.CheckFormat(DetectableEncoding.Utf32LE, true, format);
        }

        [Fact]
        public async Task Utf32LEOnlyBomShouldBeDetected()
        {
            //Arrange
            string text = "";

            //Act
            FormatSummary? format = await TextTestsHelper.EncodeAndDetectFull(text, Encoding.UTF32, DetectableEncoding.Utf32LE.BomSignature?.Value);

            //Assert
            Assert.NotNull(format);
            TextTestsHelper.CheckFormat(DetectableEncoding.Utf32LE, true, format);
        }

        [Fact]
        public async Task Utf32BEBomShouldBeDetected()
        {
            //Arrange
            string text = "Hello";

            //Act
            FormatSummary? format = await TextTestsHelper.EncodeAndDetectFull(text, TextTestsHelper.UTF32BE, DetectableEncoding.Utf32BE.BomSignature?.Value);

            //Assert
            Assert.NotNull(format);
            TextTestsHelper.CheckFormat(DetectableEncoding.Utf32BE, true, format);
        }

        [Fact]
        public async Task Utf32BEOnlyBomShouldBeDetected()
        {
            //Arrange
            string text = "";

            //Act
            FormatSummary? format = await TextTestsHelper.EncodeAndDetectFull(text, TextTestsHelper.UTF32BE, DetectableEncoding.Utf32BE.BomSignature?.Value);

            //Assert
            Assert.NotNull(format);
            TextTestsHelper.CheckFormat(DetectableEncoding.Utf32BE, true, format);
        }

        [Fact]
        public async Task Utf1BomShouldBeDetected()
        {
            //Arrange
            byte[] text = Encoding.ASCII.GetBytes("Hello"); // ASCII text instead of real encoding use

            //Act
            FormatSummary? format = await TextTestsHelper.EncodeAndDetectFull(text, DetectableEncoding.Utf1.BomSignature!.Value);

            //Assert
            Assert.NotNull(format);
            TextTestsHelper.CheckFormat(DetectableEncoding.Utf1, true, format);
        }

        [Fact]
        public async Task Utf7BomShouldBeDetected()
        {
            //Arrange
            byte[] text = Encoding.ASCII.GetBytes("Hello"); // ASCII text instead of real encoding use

            //Act
            FormatSummary? format = await TextTestsHelper.EncodeAndDetectFull(text, DetectableEncoding.Utf7.BomSignature!.Value);

            //Assert
            Assert.NotNull(format);
            TextTestsHelper.CheckFormat(DetectableEncoding.Utf7, true, format);
        }

        [Fact]
        public async Task UtfEbcdictBomShouldBeDetected()
        {
            //Arrange
            byte[] text = Encoding.ASCII.GetBytes("Hello"); // ASCII text instead of real encoding use

            //Act
            FormatSummary? format = await TextTestsHelper.EncodeAndDetectFull(text, DetectableEncoding.UtfEbcdict.BomSignature!.Value);

            //Assert
            Assert.NotNull(format);
            TextTestsHelper.CheckFormat(DetectableEncoding.UtfEbcdict, true, format);
        }

        [Fact]
        public async Task Gb18030BomShouldBeDetected()
        {
            //Arrange
            byte[] text = Encoding.ASCII.GetBytes("Hello"); // ASCII text instead of real encoding use

            //Act
            FormatSummary? format = await TextTestsHelper.EncodeAndDetectFull(text, DetectableEncoding.Gb18030.BomSignature!.Value);

            //Assert
            Assert.NotNull(format);
            TextTestsHelper.CheckFormat(DetectableEncoding.Gb18030, true, format);
        }


        [Fact]
        public async Task Bocu1BomShouldBeDetected()
        {
            //Arrange
            byte[] text = Encoding.ASCII.GetBytes("Hello"); // ASCII text instead of real encoding use

            //Act
            FormatSummary? format = await TextTestsHelper.EncodeAndDetectFull(text, DetectableEncoding.Bocu1.BomSignature!.Value);

            //Assert
            Assert.NotNull(format);
            TextTestsHelper.CheckFormat(DetectableEncoding.Bocu1, true, format);
        }

        [Fact]
        public async Task ScsuBomShouldBeDetected()
        {
            //Arrange
            byte[] text = Encoding.ASCII.GetBytes("Hello"); // ASCII text instead of real encoding use

            //Act
            FormatSummary? format = await TextTestsHelper.EncodeAndDetectFull(text, DetectableEncoding.Scsu.BomSignature!.Value);

            //Assert
            Assert.NotNull(format);
            TextTestsHelper.CheckFormat(DetectableEncoding.Scsu, true, format);
        }


    }
}
