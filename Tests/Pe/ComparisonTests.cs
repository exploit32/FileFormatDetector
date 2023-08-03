using PEFormat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Pe
{
    public class ComparisonTests
    {
        [Fact]
        public void IdenticalPEsShouldBeEquals()
        {
            //Arrange
            PEFormatSummary file1 = new PEFormatSummary("x86", 32, FormatApi.Endianness.LittleEndian, true);
            PEFormatSummary file2 = new PEFormatSummary("x86", 32, FormatApi.Endianness.LittleEndian, true);

            //Act
            //Assert
            Assert.Equal(file1, file2);
            Assert.Equal(file2, file1);
            Assert.Equal(file1.GetHashCode(), file2.GetHashCode());
        }

        [Fact]
        public void IdenticalPEsShouldBeEquals2()
        {
            //Arrange
            PEFormatSummary file1 = new PEFormatSummary("x86-64", 64, FormatApi.Endianness.BigEndian, false);
            PEFormatSummary file2 = new PEFormatSummary("x86-64", 64, FormatApi.Endianness.BigEndian, false);

            //Act
            //Assert
            Assert.Equal(file1, file2);
            Assert.Equal(file2, file1);
            Assert.Equal(file1.GetHashCode(), file2.GetHashCode());
        }

        [Fact]
        public void DifferentArchPEsShouldNotBeEquals()
        {
            //Arrange
            PEFormatSummary file1 = new PEFormatSummary("x86", 32, FormatApi.Endianness.LittleEndian, true);
            PEFormatSummary file2 = new PEFormatSummary("arm", 32, FormatApi.Endianness.LittleEndian, true);

            //Act
            //Assert
            Assert.NotEqual(file1, file2);
            Assert.NotEqual(file2, file1);
            Assert.NotEqual(file1.GetHashCode(), file2.GetHashCode());
        }

        [Fact]
        public void DifferentBitsPEsShouldNotBeEquals()
        {
            //Arrange
            PEFormatSummary file1 = new PEFormatSummary("arm", 32, FormatApi.Endianness.LittleEndian, true);
            PEFormatSummary file2 = new PEFormatSummary("arm", 64, FormatApi.Endianness.LittleEndian, true);

            //Act
            //Assert
            Assert.NotEqual(file1, file2);
            Assert.NotEqual(file2, file1);
            Assert.NotEqual(file1.GetHashCode(), file2.GetHashCode());
        }

        [Fact]
        public void DifferentEndiannessPEsShouldNotBeEquals()
        {
            //Arrange
            PEFormatSummary file1 = new PEFormatSummary("arm", 32, FormatApi.Endianness.BigEndian, true);
            PEFormatSummary file2 = new PEFormatSummary("arm", 32, FormatApi.Endianness.LittleEndian, true);

            //Act
            //Assert
            Assert.NotEqual(file1, file2);
            Assert.NotEqual(file2, file1);
            Assert.NotEqual(file1.GetHashCode(), file2.GetHashCode());
        }

        [Fact]
        public void WithClrHeaderAndWithoutClrHeaderPEsShouldNotBeEquals()
        {
            //Arrange
            PEFormatSummary file1 = new PEFormatSummary("arm", 32, FormatApi.Endianness.LittleEndian, true);
            PEFormatSummary file2 = new PEFormatSummary("arm", 32, FormatApi.Endianness.LittleEndian, false);

            //Act
            //Assert
            Assert.NotEqual(file1, file2);
            Assert.NotEqual(file2, file1);
            Assert.NotEqual(file1.GetHashCode(), file2.GetHashCode());
        }
    }
}
