using FormatApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextFilesFormat;

namespace Tests.TextFiles
{
    public class Utf16Tests
    {
        [Fact]
        public void EnglishText()
        {
            //Arrange
            string text = "Hello from utf16";

            //Act
            FormatSummary? formatLE = TextTestsHelper.EncodeAndDetectFull(text, Encoding.Unicode);
            FormatSummary? formatBE = TextTestsHelper.EncodeAndDetectFull(text, Encoding.BigEndianUnicode);

            //Assert
            Assert.NotNull(formatLE);
            TextTestsHelper.CheckFormat(DetectableEncoding.Utf16LE, false, formatLE);

            Assert.NotNull(formatBE);
            TextTestsHelper.CheckFormat(DetectableEncoding.Utf16BE, false, formatBE);
        }

        [Fact]
        public void EnglishTextWithoutSpaces()
        {
            //Arrange
            string text = "Hellofromutf16";

            //Act
            FormatSummary? formatLE = TextTestsHelper.EncodeAndDetectFull(text, Encoding.Unicode);
            FormatSummary? formatBE = TextTestsHelper.EncodeAndDetectFull(text, Encoding.BigEndianUnicode);

            //Assert
            Assert.NotNull(formatLE);
            TextTestsHelper.CheckFormat(DetectableEncoding.Utf16LE, false, formatLE);

            Assert.NotNull(formatBE);
            TextTestsHelper.CheckFormat(DetectableEncoding.Utf16BE, false, formatBE);
        }

        [Fact]
        public void ChineeseText()
        {
            //Arrange
            string text = "關於我和鬼變成家人的那件事》是一部2023年的臺灣動作喜劇電影，由程偉豪執導，許光漢、林柏宏、王淨主演；劇本由吳瑾蓉與程偉豪擔任編劇";

            //Act
            FormatSummary? formatLE = TextTestsHelper.EncodeAndDetectFull(text, Encoding.Unicode);
            FormatSummary? formatBE = TextTestsHelper.EncodeAndDetectFull(text, Encoding.BigEndianUnicode);

            //Assert
            Assert.NotNull(formatLE);
            TextTestsHelper.CheckFormat(DetectableEncoding.Utf16LE, false, formatLE);

            Assert.NotNull(formatBE);
            TextTestsHelper.CheckFormat(DetectableEncoding.Utf16BE, false, formatBE);
        }

        [Fact]
        public void MixedTextWithoutSpaces()
        {
            //Arrange
            string text = "HelloТест";

            //Act
            FormatSummary? formatLE = TextTestsHelper.EncodeAndDetectFull(text, Encoding.Unicode);
            FormatSummary? formatBE = TextTestsHelper.EncodeAndDetectFull(text, Encoding.BigEndianUnicode);

            //Assert
            Assert.NotNull(formatLE);
            TextTestsHelper.CheckFormat(DetectableEncoding.Utf16LE, false, formatLE);

            Assert.NotNull(formatBE);
            TextTestsHelper.CheckFormat(DetectableEncoding.Utf16BE, false, formatBE);
        }

        [Fact]
        public void EnglishAndRussianText()
        {
            //Arrange
            string text = "Hello from utf32. Привет, это UTF-16";

            //Act
            FormatSummary? formatLE = TextTestsHelper.EncodeAndDetectFull(text, Encoding.Unicode);
            FormatSummary? formatBE = TextTestsHelper.EncodeAndDetectFull(text, Encoding.BigEndianUnicode);

            //Assert
            Assert.NotNull(formatLE);
            TextTestsHelper.CheckFormat(DetectableEncoding.Utf16LE, false, formatLE);

            Assert.NotNull(formatBE);
            TextTestsHelper.CheckFormat(DetectableEncoding.Utf16BE, false, formatBE);
        }

        [Fact]
        public void RussianTextWithoutSpaces()
        {
            //Arrange
            string text = "Привет";

            //Act
            FormatSummary? formatLE = TextTestsHelper.EncodeAndDetectFull(text, Encoding.Unicode);
            FormatSummary? formatBE = TextTestsHelper.EncodeAndDetectFull(text, Encoding.BigEndianUnicode);

            //Assert
            Assert.NotNull(formatLE);
            TextTestsHelper.CheckFormat(DetectableEncoding.Utf16LE, false, formatLE);

            Assert.NotNull(formatBE);
            TextTestsHelper.CheckFormat(DetectableEncoding.Utf16BE, false, formatBE);
        }

        [Fact]
        public void Emojies()
        {
            //Arrange
            string text = "🐕💉💉💉";

            //Act
            FormatSummary? formatLE = TextTestsHelper.EncodeAndDetectFull(text, Encoding.Unicode);
            FormatSummary? formatBE = TextTestsHelper.EncodeAndDetectFull(text, Encoding.BigEndianUnicode);

            //Assert
            Assert.NotNull(formatLE);
            TextTestsHelper.CheckFormat(DetectableEncoding.Utf16LE, false, formatLE);

            Assert.NotNull(formatBE);
            TextTestsHelper.CheckFormat(DetectableEncoding.Utf16BE, false, formatBE);
        }

        [Fact]
        public void RegularTextAndEmojies()
        {
            //Arrange
            string text = "Hello 🐕💉💉💉";

            //Act
            FormatSummary? formatLE = TextTestsHelper.EncodeAndDetectFull(text, Encoding.Unicode);
            FormatSummary? formatBE = TextTestsHelper.EncodeAndDetectFull(text, Encoding.BigEndianUnicode);

            //Assert
            Assert.NotNull(formatLE);
            TextTestsHelper.CheckFormat(DetectableEncoding.Utf16LE, false, formatLE);

            Assert.NotNull(formatBE);
            TextTestsHelper.CheckFormat(DetectableEncoding.Utf16BE, false, formatBE);
        }

        [Fact]
        public void NullSymboldShouldNotBeRecognized()
        {
            //Arrange
            string text = "Hello \0 World!";

            //Act
            FormatSummary? formatLE = TextTestsHelper.EncodeAndDetectFull(text, Encoding.Unicode);
            FormatSummary? formatBE = TextTestsHelper.EncodeAndDetectFull(text, Encoding.BigEndianUnicode);

            //Assert
            Assert.Null(formatLE);
            Assert.Null(formatBE);
        }
    }
}
