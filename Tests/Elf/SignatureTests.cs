using ElfFormat;

namespace Tests.Elf
{
    public class SignatureTests
    {
        [Theory]
        [InlineData(new byte[] { 0x7F, (byte)'E', (byte)'L', (byte)'F', 0x0, 0x0 })]
        [InlineData(new byte[] { 0x7F, (byte)'E', (byte)'L', (byte)'F' })]
        public void CorrectElfMagicShouldBeFound(byte[] magic)
        {
            //Arrange
            ElfFormatDetector detector = new ElfFormatDetector();

            //Act
            bool signatureFound = detector.CheckSignature(magic.AsSpan());

            //Assert
            Assert.True(signatureFound);
        }

        [Theory]
        [InlineData(new byte[] { (byte)'M', (byte)'Z', 0x0, 0x0, 0x0, 0x0 })]
        [InlineData(new byte[] { 0x7F, (byte)'E', (byte)'L' })]
        [InlineData(new byte[] { })]
        [InlineData(new byte[] { 0xfe, 0xed, 0xfa, 0xce })]
        public void InvalidMagicShouldBeIgnored(byte[] magic)
        {
            //Arrange
            ElfFormatDetector detector = new ElfFormatDetector();

            //Act
            bool signatureFound = detector.CheckSignature(magic.AsSpan());

            //Assert
            Assert.False(signatureFound);
        }
    }
}
