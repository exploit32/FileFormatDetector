using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextFilesFormat.Checkers;

namespace Tests.TextFiles
{
    public class Utf16CheckerTests
    {
        [Fact]
        public void NullSymbolsShouldNotBeValid()
        {
            //Arrange
            Utf16Checker checker = new Utf16Checker();

            //Act
            bool correct = checker.CheckValidRange(new byte[] { 0x0, 0x0, 0x1, 0x1 });

            //Assert
            Assert.False(correct);
        }

        [Fact]
        public void BigEndianSurrogateShouldBeFound()
        {
            //Arrange
            Utf16Checker checker = new Utf16Checker();

            //Act
            bool correct = checker.CheckValidRange(new byte[] { 0xD8, 0x00, 0xDC, 0x00});

            //Assert
            Assert.True(correct);
            Assert.True(checker.BigEndianSurrogatesValid);
            Assert.True(checker.LittleEndianSurrogatesValid);
            Assert.True(checker.FoundBigEndianSurrogates);
            Assert.False(checker.FoundLittleEndianSurrogates);
        }

        [Fact]
        public void InvalidBigEndianSurrogateShouldBeDetected()
        {
            //Arrange
            Utf16Checker checker = new Utf16Checker();

            //Act
            bool correct = checker.CheckValidRange(new byte[] { 0xD8, 0x00, 0x01, 0x01 });

            //Assert
            Assert.True(correct);
            Assert.False(checker.BigEndianSurrogatesValid);
            Assert.True(checker.LittleEndianSurrogatesValid);
            Assert.False(checker.FoundBigEndianSurrogates);
            Assert.False(checker.FoundLittleEndianSurrogates);
        }

        [Fact]
        public void LittleEndianSurrogateShouldBeFound()
        {
            //Arrange
            Utf16Checker checker = new Utf16Checker();

            //Act
            bool correct = checker.CheckValidRange(new byte[] { 0x00, 0xD8, 0x00, 0xDC });

            //Assert
            Assert.True(correct);
            Assert.True(checker.BigEndianSurrogatesValid);
            Assert.True(checker.LittleEndianSurrogatesValid);
            Assert.False(checker.FoundBigEndianSurrogates);
            Assert.True(checker.FoundLittleEndianSurrogates);
        }

        [Fact]
        public void InvalidLittleEndianSurrogateShouldBeDetected()
        {
            //Arrange
            Utf16Checker checker = new Utf16Checker();

            //Act
            bool correct = checker.CheckValidRange(new byte[] { 0x00, 0xD8, 0x01, 0x01 });

            //Assert
            Assert.True(correct);
            Assert.True(checker.BigEndianSurrogatesValid);
            Assert.False(checker.LittleEndianSurrogatesValid);
            Assert.False(checker.FoundBigEndianSurrogates);
            Assert.False(checker.FoundLittleEndianSurrogates);
        }

        [Fact]
        public void NumberOfEvenBytesShouldBeCalculated()
        {
            //Arrange
            Utf16Checker checker = new Utf16Checker();

            //Act
            bool correct = checker.CheckValidRange(new byte[] { 0x1, 0x0, 0x2, 0x0, 0x3, 0x0, 0x3, 0x0 });

            //Assert
            Assert.True(correct);
            Assert.True(checker.BigEndianSurrogatesValid);
            Assert.True(checker.LittleEndianSurrogatesValid);
            Assert.False(checker.FoundBigEndianSurrogates);
            Assert.False(checker.FoundLittleEndianSurrogates);

            var distinctBytes = checker.GetDistinctBytes();
            Assert.Equal(3, distinctBytes.uniqueEvenBytes);
            Assert.Equal(1, distinctBytes.uniqueOddBytes);
        }

        [Fact]
        public void NumberOfOddBytesShouldBeCalculated()
        {
            //Arrange
            Utf16Checker checker = new Utf16Checker();

            //Act
            bool correct = checker.CheckValidRange(new byte[] { 0x0, 0x1, 0x0, 0x2, 0x0, 0x3, 0x0, 0x3 });

            //Assert
            Assert.True(correct);
            Assert.True(checker.BigEndianSurrogatesValid);
            Assert.True(checker.LittleEndianSurrogatesValid);
            Assert.False(checker.FoundBigEndianSurrogates);
            Assert.False(checker.FoundLittleEndianSurrogates);

            var distinctBytes = checker.GetDistinctBytes();
            Assert.Equal(1, distinctBytes.uniqueEvenBytes);
            Assert.Equal(3, distinctBytes.uniqueOddBytes);
        }
    }
}
