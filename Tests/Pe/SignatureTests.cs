using PEFormat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Pe
{
    public class SignatureTests
    {
        [Theory]
        [InlineData(new byte[] { (byte)'M', (byte)'Z', 0x0, 0x0, 0x0, 0x0 })]
        [InlineData(new byte[] { (byte)'M', (byte)'Z' })]
        [InlineData(new byte[] { (byte)'M', (byte)'Z', 0xFF })]
        public void MZMagic(byte[] magic)
        {
            //Arrange
            PEFormatDetector detector = new PEFormatDetector();

            //Act
            bool signatureFound = detector.CheckSignature(magic.AsSpan());

            //Assert
            Assert.True(signatureFound);
        }

        [Theory]
        [InlineData(new byte[] { (byte)'X', (byte)'Z', 0x0, 0x0, 0x0, 0x0 })]
        [InlineData(new byte[] { (byte)'M', })]
        [InlineData(new byte[] { })]
        [InlineData(new byte[] { 0xfe, 0xed, 0xfa, 0xce })]
        public void InvalidMagic(byte[] magic)
        {
            //Arrange
            PEFormatDetector detector = new PEFormatDetector();

            //Act
            bool signatureFound = detector.CheckSignature(magic.AsSpan());

            //Assert
            Assert.False(signatureFound);
        }
    }
}
