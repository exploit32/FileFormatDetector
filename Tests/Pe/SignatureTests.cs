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
        [Fact]
        public void MZMagic()
        {
            //Arrange
            PEFormatDetector detector = new PEFormatDetector();

            byte[] signature = new byte[] { (byte)'M', (byte)'Z', 0x0, 0x0, 0x0, 0x0 };

            //Act
            bool signatureFound = detector.CheckSignature(signature.AsSpan());
            
            //Assert
            Assert.True(signatureFound);
        }

        [Fact]
        public void EmptyMagic()
        {
            //Arrange
            PEFormatDetector detector = new PEFormatDetector();

            byte[] signature = new byte[0];

            //Act
            bool signatureFound = detector.CheckSignature(signature.AsSpan());

            //Assert
            Assert.False(signatureFound);
        }

        [Fact]
        public void InvalidMagic()
        {
            //Arrange
            PEFormatDetector detector = new PEFormatDetector();

            byte[] signature = new byte[] { (byte)'X', (byte)'Z', 0x0, 0x0, 0x0, 0x0 };

            //Act
            bool signatureFound = detector.CheckSignature(signature.AsSpan());

            //Assert
            Assert.False(signatureFound);
        }
    }
}
