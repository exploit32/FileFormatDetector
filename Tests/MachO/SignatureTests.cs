using MachOFormat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.MachO
{
    public class SignatureTests
    {
        [Theory]
        [InlineData(new byte[] { 0xce, 0xfa, 0xed, 0xfe })]
        [InlineData(new byte[] { 0xfe, 0xed, 0xfa, 0xce })]
        [InlineData(new byte[] { 0xcf, 0xfa, 0xed, 0xfe })]
        [InlineData(new byte[] { 0xfe, 0xed, 0xfa, 0xcf })]
        [InlineData(new byte[] { 0xca, 0xfe, 0xba, 0xbe })]
        [InlineData(new byte[] { 0xca, 0xfe, 0xba, 0xbf })]
        [InlineData(new byte[] { 0xce, 0xfa, 0xed, 0xfe, 0, 0 })]
        public void CheckMagic(byte[] magic)
        {
            //Arrange
            MachOFormatDetector detector = new MachOFormatDetector();

            //Act
            bool signatureFound = detector.CheckSignature(magic.AsSpan());

            //Assert
            Assert.True(signatureFound);
        }

        [Theory]
        [InlineData(new byte[] { 0xce, 0xfa, })]
        [InlineData(new byte[] { 0xfe, })]
        [InlineData(new byte[] { })]
        [InlineData(new byte[] { 0x7F, (byte)'E', (byte)'L', (byte)'F' })]
        public void CheckInvalidMagic(byte[] magic)
        {
            //Arrange
            MachOFormatDetector detector = new MachOFormatDetector();

            //Act
            bool signatureFound = detector.CheckSignature(magic.AsSpan());

            //Assert
            Assert.False(signatureFound);
        }
    }
}
