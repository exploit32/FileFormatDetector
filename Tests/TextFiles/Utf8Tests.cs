using FormatApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextFilesFormat;

namespace Tests.TextFiles
{
    public class Utf8Tests
    {
        [Fact]
        public void TestRussianText()
        {
            //Arrange
            string text = "Приветики";

            //Act
            FormatSummary? format = TextTestsHelper.EncodeAndDetectFull(text, Encoding.UTF8);

            //Assert
            Assert.NotNull(format);
            TextTestsHelper.CheckFormat(DetectableEncoding.Utf8, false, format);
        }

        [Fact]
        public void TestRussianTextWithEvenTextLength()
        {
            //Arrange
            string text = "Привет";

            //Act
            FormatSummary? format = TextTestsHelper.EncodeAndDetectFull(text, Encoding.UTF8);

            //Assert
            Assert.NotNull(format);
            TextTestsHelper.CheckFormat(DetectableEncoding.Utf8, false, format);
        }

        [Fact]
        public void TestEnglishAndRussianText()
        {
            //Arrange
            string text = "Hello, Hi\nПривет!";

            //Act
            FormatSummary? format = TextTestsHelper.EncodeAndDetectFull(text, Encoding.UTF8);

            //Assert
            Assert.NotNull(format);
            TextTestsHelper.CheckFormat(DetectableEncoding.Utf8, false, format);
        }

        [Fact]
        public void MixedTextWithoutSpaces()
        {
            //Arrange
            string text = "HelloПривет";

            //Act
            FormatSummary? format = TextTestsHelper.EncodeAndDetectFull(text, Encoding.UTF8);

            //Assert
            Assert.NotNull(format);
            TextTestsHelper.CheckFormat(DetectableEncoding.Utf8, false, format);
        }

        [Fact]
        public void ChineeseText()
        {
            //Arrange
            string text = "關於我和鬼變成家人的那件事》是一部2023年的臺灣動作喜劇電影，由程偉豪執導，許光漢、林柏宏、王淨主演；劇本由吳瑾蓉與程偉豪擔任編劇";

            //Act
            FormatSummary? format = TextTestsHelper.EncodeAndDetectFull(text, Encoding.UTF8);

            //Assert
            Assert.NotNull(format);
            TextTestsHelper.CheckFormat(DetectableEncoding.Utf8, false, format);
        }

        [Fact]
        public void Emojies()
        {
            //Arrange
            string text = "🐕💉💉💉";

            //Act
            FormatSummary? format = TextTestsHelper.EncodeAndDetectFull(text, Encoding.UTF8);

            //Assert
            Assert.NotNull(format);
            TextTestsHelper.CheckFormat(DetectableEncoding.Utf8, false, format);
        }
    }
}
