using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextFilesFormat.Checkers;

namespace Tests.TextFiles
{
    public class Utf8CheckerTests
    {
        [Fact]
        public void NullSymbolsShouldNotBeValid()
        {
            //Arrange
            Utf8Checker checker = new Utf8Checker();

            //Act
            bool correct = checker.CheckSurrogates(new byte[] { 0x0, 0x1, 0x1, 0x1 });

            //Assert
            Assert.False(correct);
        }

        [Fact]
        public void AsciiRangeShouldBeConsideredAsValid()
        {
            //Arrange
            Utf8Checker checker = new Utf8Checker();

            //Act
            bool correct = checker.CheckSurrogates(new byte[] { (byte)'H', (byte)'e', (byte)'l', (byte)'l', (byte)'o', (byte)'!' });

            //Assert
            Assert.True(correct);
            Assert.True(checker.SurrogatesValid);
            Assert.False(checker.FoundSurrogates);
        }

        [Fact]
        public void TwoByteSurrogatesShouldBeDetected()
        {
            //Arrange
            Utf8Checker checker = new Utf8Checker();

            //Act
            bool correct = checker.CheckSurrogates(new byte[] { (byte)'T', (byte)'e', (byte)'s', (byte)'t', 0xC2, 0xA9 });

            //Assert
            Assert.True(correct);
            Assert.True(checker.SurrogatesValid);
            Assert.True(checker.FoundSurrogates);
        }


        [Fact]
        public void ThreeByteSurrogatesShouldBeDetected()
        {
            //Arrange
            Utf8Checker checker = new Utf8Checker();

            //Act
            bool correct = checker.CheckSurrogates(new byte[] { (byte)'T', (byte)'e', (byte)'s', (byte)'t', 0xE3, 0x98, 0x83 });

            //Assert
            Assert.True(correct);
            Assert.True(checker.SurrogatesValid);
            Assert.True(checker.FoundSurrogates);
        }

        [Fact]
        public void FourByteSurrogatesShouldBeDetected()
        {
            //Arrange
            Utf8Checker checker = new Utf8Checker();

            //Act
            bool correct = checker.CheckSurrogates(new byte[] { (byte)'T', (byte)'e', (byte)'s', (byte)'t', 0xF0, 0x9D, 0x8C, 0x86 });

            //Assert
            Assert.True(correct);
            Assert.True(checker.SurrogatesValid);
            Assert.True(checker.FoundSurrogates);
        }

        [Fact]
        public void SeparatedFourByteSurrogatesShouldBeDetected()
        {
            //Arrange
            Utf8Checker checker = new Utf8Checker();

            //Act
            bool correct = checker.CheckSurrogates(new byte[] { (byte)'T', (byte)'e', (byte)'s', (byte)'t', 0xF0, 0x9D });

            //Assert
            Assert.True(correct);
            Assert.True(checker.SurrogatesValid);
            Assert.True(checker.FoundSurrogates);

            //Act2
            bool correct2 = checker.CheckSurrogates(new byte[] { 0x8C, 0x86 });

            //Assert2
            Assert.True(correct2);
            Assert.True(checker.SurrogatesValid);
            Assert.True(checker.FoundSurrogates);
        }

        [Fact]
        public void InvalidSeparatedFourByteSurrogatesShouldBeDetected()
        {
            //Arrange
            Utf8Checker checker = new Utf8Checker();

            //Act
            bool correct = checker.CheckSurrogates(new byte[] { (byte)'T', (byte)'e', (byte)'s', (byte)'t', 0xF0, 0x9D });

            //Assert
            Assert.True(correct);
            Assert.True(checker.SurrogatesValid);
            Assert.True(checker.FoundSurrogates);

            //Act2
            bool correct2 = checker.CheckSurrogates(new byte[] { 0x20, 0x86 });

            //Assert2
            Assert.False(correct2);
            Assert.False(checker.SurrogatesValid);
            Assert.True(checker.FoundSurrogates);
        }
    }
}
