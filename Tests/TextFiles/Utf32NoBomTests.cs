using FormatApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextFilesFormat;

namespace Tests.TextFiles
{
    public class Utf32NoBomTests
    {
        [Fact]
        public async Task EnglishTextShouldBeDetected()
        {
            //Arrange
            string text = "Hello from utf32";

            //Act
            FormatSummary? formatLE = await TextTestsHelper.EncodeAndDetectFull(text, Encoding.UTF32);
            FormatSummary? formatBE = await TextTestsHelper.EncodeAndDetectFull(text, TextTestsHelper.UTF32BE);

            //Assert
            Assert.NotNull(formatLE);
            TextTestsHelper.CheckFormat(DetectableEncoding.Utf32LE, false, formatLE);

            Assert.NotNull(formatBE);
            TextTestsHelper.CheckFormat(DetectableEncoding.Utf32BE, false, formatBE);
        }

        [Fact]
        public async Task EnglishAndRussianTextShouldBeDetected()
        {
            //Arrange
            string text = "Hello from utf32. Привет, это UTF-32";

            //Act
            FormatSummary? formatLE = await TextTestsHelper.EncodeAndDetectFull(text, Encoding.UTF32);
            FormatSummary? formatBE = await TextTestsHelper.EncodeAndDetectFull(text, TextTestsHelper.UTF32BE);

            //Assert
            Assert.NotNull(formatLE);
            TextTestsHelper.CheckFormat(DetectableEncoding.Utf32LE, false, formatLE);

            Assert.NotNull(formatBE);
            TextTestsHelper.CheckFormat(DetectableEncoding.Utf32BE, false, formatBE);
        }

        [Fact]
        public async Task EmojiesShouldBeDetected()
        {
            //Arrange
            string text = "🐕💉💉💉";

            //Act
            FormatSummary? formatLE = await TextTestsHelper.EncodeAndDetectFull(text, Encoding.UTF32);
            FormatSummary? formatBE = await TextTestsHelper.EncodeAndDetectFull(text, TextTestsHelper.UTF32BE);

            //Assert
            Assert.NotNull(formatLE);
            TextTestsHelper.CheckFormat(DetectableEncoding.Utf32LE, false, formatLE);

            Assert.NotNull(formatBE);
            TextTestsHelper.CheckFormat(DetectableEncoding.Utf32BE, false, formatBE);
        }

        [Fact]
        public async Task RegularTextAndEmojiesShouldBeDetected()
        {
            //Arrange
            string text = "Hello 🐕💉💉💉";

            //Act
            FormatSummary? formatLE = await TextTestsHelper.EncodeAndDetectFull(text, Encoding.UTF32);
            FormatSummary? formatBE = await TextTestsHelper.EncodeAndDetectFull(text, TextTestsHelper.UTF32BE);

            //Assert
            Assert.NotNull(formatLE);
            TextTestsHelper.CheckFormat(DetectableEncoding.Utf32LE, false, formatLE);

            Assert.NotNull(formatBE);
            TextTestsHelper.CheckFormat(DetectableEncoding.Utf32BE, false, formatBE);
        }

        [Fact]
        public async Task NullSumbolsShouldNotBeRecognized()
        {
            //Arrange
            string text = "Hello \0 world!";

            //Act
            FormatSummary? formatLE = await TextTestsHelper.EncodeAndDetectFull(text, Encoding.UTF32);
            FormatSummary? formatBE = await TextTestsHelper.EncodeAndDetectFull(text, TextTestsHelper.UTF32BE);

            //Assert
            Assert.Null(formatLE);
            Assert.Null(formatBE);
        }
    }
}
