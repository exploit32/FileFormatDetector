﻿using FormatApi;
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
        public async Task EnglishText()
        {
            //Arrange
            string text = "Hello from utf16";

            //Act
            FormatSummary? formatLE = await TextTestsHelper.EncodeAndDetectFull(text, Encoding.Unicode);
            FormatSummary? formatBE = await TextTestsHelper.EncodeAndDetectFull(text, Encoding.BigEndianUnicode);

            //Assert
            Assert.NotNull(formatLE);
            TextTestsHelper.CheckFormat(DetectableEncoding.Utf16LE, false, formatLE);

            Assert.NotNull(formatBE);
            TextTestsHelper.CheckFormat(DetectableEncoding.Utf16BE, false, formatBE);
        }

        [Fact]
        public async Task EnglishTextWithoutSpaces()
        {
            //Arrange
            string text = "Hellofromutf16";

            //Act
            FormatSummary? formatLE = await TextTestsHelper.EncodeAndDetectFull(text, Encoding.Unicode);
            FormatSummary? formatBE = await TextTestsHelper.EncodeAndDetectFull(text, Encoding.BigEndianUnicode);

            //Assert
            Assert.NotNull(formatLE);
            TextTestsHelper.CheckFormat(DetectableEncoding.Utf16LE, false, formatLE);

            Assert.NotNull(formatBE);
            TextTestsHelper.CheckFormat(DetectableEncoding.Utf16BE, false, formatBE);
        }

        [Fact]
        public async Task ChineeseText()
        {
            //Arrange
            string text = "關於我和鬼變成家人的那件事》是一部2023年的臺灣動作喜劇電影，由程偉豪執導，許光漢、林柏宏、王淨主演；劇本由吳瑾蓉與程偉豪擔任編劇";

            //Act
            FormatSummary? formatLE = await TextTestsHelper.EncodeAndDetectFull(text, Encoding.Unicode);
            FormatSummary? formatBE = await TextTestsHelper.EncodeAndDetectFull(text, Encoding.BigEndianUnicode);

            //Assert
            Assert.NotNull(formatLE);
            TextTestsHelper.CheckFormat(DetectableEncoding.Utf16LE, false, formatLE);

            Assert.NotNull(formatBE);
            TextTestsHelper.CheckFormat(DetectableEncoding.Utf16BE, false, formatBE);
        }

        [Fact]
        public async Task MixedTextWithoutSpaces()
        {
            //Arrange
            string text = "HelloТест";

            //Act
            FormatSummary? formatLE = await TextTestsHelper.EncodeAndDetectFull(text, Encoding.Unicode);
            FormatSummary? formatBE = await TextTestsHelper.EncodeAndDetectFull(text, Encoding.BigEndianUnicode);

            //Assert
            Assert.NotNull(formatLE);
            TextTestsHelper.CheckFormat(DetectableEncoding.Utf16LE, false, formatLE);

            Assert.NotNull(formatBE);
            TextTestsHelper.CheckFormat(DetectableEncoding.Utf16BE, false, formatBE);
        }

        [Fact]
        public async Task EnglishAndRussianText()
        {
            //Arrange
            string text = "Hello from utf32. Привет, это UTF-16";

            //Act
            FormatSummary? formatLE = await TextTestsHelper.EncodeAndDetectFull(text, Encoding.Unicode);
            FormatSummary? formatBE = await TextTestsHelper.EncodeAndDetectFull(text, Encoding.BigEndianUnicode);

            //Assert
            Assert.NotNull(formatLE);
            TextTestsHelper.CheckFormat(DetectableEncoding.Utf16LE, false, formatLE);

            Assert.NotNull(formatBE);
            TextTestsHelper.CheckFormat(DetectableEncoding.Utf16BE, false, formatBE);
        }

        [Fact]
        public async Task RussianTextWithoutSpaces()
        {
            //Arrange
            string text = "Привет";

            //Act
            FormatSummary? formatLE = await TextTestsHelper.EncodeAndDetectFull(text, Encoding.Unicode);
            FormatSummary? formatBE = await TextTestsHelper.EncodeAndDetectFull(text, Encoding.BigEndianUnicode);

            //Assert
            Assert.NotNull(formatLE);
            TextTestsHelper.CheckFormat(DetectableEncoding.Utf16LE, false, formatLE);

            Assert.NotNull(formatBE);
            TextTestsHelper.CheckFormat(DetectableEncoding.Utf16BE, false, formatBE);
        }

        [Fact]
        public async Task Emojies()
        {
            //Arrange
            string text = "🐕💉💉💉";

            //Act
            FormatSummary? formatLE = await TextTestsHelper.EncodeAndDetectFull(text, Encoding.Unicode);
            FormatSummary? formatBE = await TextTestsHelper.EncodeAndDetectFull(text, Encoding.BigEndianUnicode);

            //Assert
            Assert.NotNull(formatLE);
            TextTestsHelper.CheckFormat(DetectableEncoding.Utf16LE, false, formatLE);

            Assert.NotNull(formatBE);
            TextTestsHelper.CheckFormat(DetectableEncoding.Utf16BE, false, formatBE);
        }

        [Fact]
        public async Task RegularTextAndEmojies()
        {
            //Arrange
            string text = "Hello 🐕💉💉💉";

            //Act
            FormatSummary? formatLE = await TextTestsHelper.EncodeAndDetectFull(text, Encoding.Unicode);
            FormatSummary? formatBE = await TextTestsHelper.EncodeAndDetectFull(text, Encoding.BigEndianUnicode);

            //Assert
            Assert.NotNull(formatLE);
            TextTestsHelper.CheckFormat(DetectableEncoding.Utf16LE, false, formatLE);

            Assert.NotNull(formatBE);
            TextTestsHelper.CheckFormat(DetectableEncoding.Utf16BE, false, formatBE);
        }

        [Fact]
        public async Task NullSymboldShouldNotBeRecognized()
        {
            //Arrange
            string text = "Hello \0 World!";

            //Act
            FormatSummary? formatLE = await TextTestsHelper.EncodeAndDetectFull(text, Encoding.Unicode);
            FormatSummary? formatBE = await TextTestsHelper.EncodeAndDetectFull(text, Encoding.BigEndianUnicode);

            //Assert
            Assert.Null(formatLE);
            Assert.Null(formatBE);
        }
    }
}
