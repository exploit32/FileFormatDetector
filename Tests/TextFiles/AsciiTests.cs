using FormatApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextFilesFormat;

namespace Tests.TextFiles
{
    public class AsciiTests
    {
        [Fact]
        public void Simple()
        {
            //Arrange
            string text = "This is simple text";

            //Act
            FormatSummary? format = TextTestsHelper.EncodeAndDetectFull(text, Encoding.ASCII);

            //Assert
            Assert.NotNull(format);
            TextTestsHelper.CheckFormat(DetectableEncoding.ASCII, false, format);
        }

        [Fact]
        public void Multiline()
        {
            //Arrange
            string text = "This is simple text\r\nwith several lines";

            //Act
            FormatSummary? format = TextTestsHelper.EncodeAndDetectFull(text, Encoding.ASCII);

            //Assert
            Assert.NotNull(format);
            TextTestsHelper.CheckFormat(DetectableEncoding.ASCII, false, format);
        }

        [Fact]
        public void MultilineWithTab()
        {
            //Arrange
            string text = "This is simple text\r\nwith several\tlines";

            //Act
            FormatSummary? format = TextTestsHelper.EncodeAndDetectFull(text, Encoding.ASCII);

            //Assert
            Assert.NotNull(format);
            TextTestsHelper.CheckFormat(DetectableEncoding.ASCII, false, format);
        }

        [Fact]
        public void NullsShouldNotBeAscii()
        {
            //Arrange
            string text = "Wait, oh shi\0\0";

            //Act
            FormatSummary? format = TextTestsHelper.EncodeAndDetectFull(text, Encoding.ASCII);

            //Assert
            Assert.Null(format);
        }
    }
}
