using FormatApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextFilesFormat;

namespace Tests.TextFiles
{
    public class Utf32Tests
    {
        [Fact]
        public void EnglishText()
        {
            //Arrange
            string text = "Hello from utf32";

            //Act
            FormatSummary? formatLE = TextTestsHelper.EncodeAndDetectFull(text, Encoding.UTF32);
            FormatSummary? formatBE = TextTestsHelper.EncodeAndDetectFull(text, TextTestsHelper.UTF32BE);

            //Assert
            Assert.NotNull(formatLE);
            TextTestsHelper.CheckFormat(DetectableEncoding.Utf32LE, false, formatLE);

            Assert.NotNull(formatBE);
            TextTestsHelper.CheckFormat(DetectableEncoding.Utf32BE, false, formatBE);
        }

        [Fact]
        public void EnglishAndRussianText()
        {
            //Arrange
            string text = "Hello from utf32. Привет, это UTF-32";

            //Act
            FormatSummary? formatLE = TextTestsHelper.EncodeAndDetectFull(text, Encoding.UTF32);
            FormatSummary? formatBE = TextTestsHelper.EncodeAndDetectFull(text, TextTestsHelper.UTF32BE);

            //Assert
            Assert.NotNull(formatLE);
            TextTestsHelper.CheckFormat(DetectableEncoding.Utf32LE, false, formatLE);

            Assert.NotNull(formatBE);
            TextTestsHelper.CheckFormat(DetectableEncoding.Utf32BE, false, formatBE);
        }

        [Fact]
        public void Emojies()
        {
            //Arrange
            string text = "🐕💉💉💉";

            //Act
            FormatSummary? formatLE = TextTestsHelper.EncodeAndDetectFull(text, Encoding.UTF32);
            FormatSummary? formatBE = TextTestsHelper.EncodeAndDetectFull(text, TextTestsHelper.UTF32BE);

            //Assert
            Assert.NotNull(formatLE);
            TextTestsHelper.CheckFormat(DetectableEncoding.Utf32LE, false, formatLE);

            Assert.NotNull(formatBE);
            TextTestsHelper.CheckFormat(DetectableEncoding.Utf32BE, false, formatBE);
        }

        [Fact]
        public void RegularTextAndEmojies()
        {
            //Arrange
            string text = "Hello 🐕💉💉💉";

            //Act
            FormatSummary? formatLE = TextTestsHelper.EncodeAndDetectFull(text, Encoding.UTF32);
            FormatSummary? formatBE = TextTestsHelper.EncodeAndDetectFull(text, TextTestsHelper.UTF32BE);

            //Assert
            Assert.NotNull(formatLE);
            TextTestsHelper.CheckFormat(DetectableEncoding.Utf32LE, false, formatLE);

            Assert.NotNull(formatBE);
            TextTestsHelper.CheckFormat(DetectableEncoding.Utf32BE, false, formatBE);
        }

        [Fact]
        public void NullSumbolsShouldNotBeRecognized()
        {
            //Arrange
            string text = "Hello \0 world!";

            //Act
            FormatSummary? formatLE = TextTestsHelper.EncodeAndDetectFull(text, Encoding.UTF32);
            FormatSummary? formatBE = TextTestsHelper.EncodeAndDetectFull(text, TextTestsHelper.UTF32BE);

            //Assert
            Assert.Null(formatLE);
            Assert.Null(formatBE);
        }
    }
}
