using ElfFormat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Elf
{
    public class ComparisonTests
    {
        [Fact]
        public void IdenticalElfsShouldBeEqual()
        {
            //Arrange
            ElfFormatSummary file1 = new ElfFormatSummary("x86", 32, FormatApi.Endianness.LittleEndian, "/lib/ld-linux.so.2");
            ElfFormatSummary file2 = new ElfFormatSummary("x86", 32, FormatApi.Endianness.LittleEndian, "/lib/ld-linux.so.2");

            //Act
            //Assert
            Assert.Equal(file1, file2);
            Assert.Equal(file2, file1);
            Assert.Equal(file1.GetHashCode(), file2.GetHashCode());
        }

        [Fact]
        public void IdenticalElfsShouldBeEqual2()
        {
            //Arrange
            ElfFormatSummary file1 = new ElfFormatSummary("x64", 64, FormatApi.Endianness.BigEndian, "/lib/ld-linux.so.2");
            ElfFormatSummary file2 = new ElfFormatSummary("x64", 64, FormatApi.Endianness.BigEndian, "/lib/ld-linux.so.2");

            //Act
            //Assert
            Assert.Equal(file1, file2);
            Assert.Equal(file2, file1);
            Assert.Equal(file1.GetHashCode(), file2.GetHashCode());
        }

        [Fact]
        public void DifferentArchElfsShouldNotBeEqual()
        {
            //Arrange
            ElfFormatSummary file1 = new ElfFormatSummary("x64", 64, FormatApi.Endianness.BigEndian, "/lib/ld-linux.so.2");
            ElfFormatSummary file2 = new ElfFormatSummary("arm", 64, FormatApi.Endianness.BigEndian, "/lib/ld-linux.so.2");

            //Act
            //Assert
            Assert.NotEqual(file1, file2);
            Assert.NotEqual(file2, file1);
            Assert.NotEqual(file1.GetHashCode(), file2.GetHashCode());
        }

        [Fact]
        public void DifferentBitsElfsShouldNotBeEqual()
        {
            //Arrange
            ElfFormatSummary file1 = new ElfFormatSummary("arm", 32, FormatApi.Endianness.BigEndian, "/lib/ld-linux.so.2");
            ElfFormatSummary file2 = new ElfFormatSummary("arm", 64, FormatApi.Endianness.BigEndian, "/lib/ld-linux.so.2");

            //Act
            //Assert
            Assert.NotEqual(file1, file2);
            Assert.NotEqual(file2, file1);
            Assert.NotEqual(file1.GetHashCode(), file2.GetHashCode());
        }

        [Fact]
        public void DifferentEndiannessElfsShouldNotBeEqual()
        {
            //Arrange
            ElfFormatSummary file1 = new ElfFormatSummary("arm", 64, FormatApi.Endianness.LittleEndian, "/lib/ld-linux.so.2");
            ElfFormatSummary file2 = new ElfFormatSummary("arm", 64, FormatApi.Endianness.BigEndian, "/lib/ld-linux.so.2");

            //Act
            //Assert
            Assert.NotEqual(file1, file2);
            Assert.NotEqual(file2, file1);
            Assert.NotEqual(file1.GetHashCode(), file2.GetHashCode());
        }

        [Fact]
        public void DifferentEnterpretersElfsShouldNotBeEqual()
        {
            //Arrange
            ElfFormatSummary file1 = new ElfFormatSummary("arm", 32, FormatApi.Endianness.LittleEndian, "/lib/ld-linux.so.2");
            ElfFormatSummary file2 = new ElfFormatSummary("arm", 32, FormatApi.Endianness.LittleEndian, "/usr/lib/libc.so.1");

            //Act
            //Assert
            Assert.NotEqual(file1, file2);
            Assert.NotEqual(file2, file1);
            Assert.NotEqual(file1.GetHashCode(), file2.GetHashCode());
        }

        [Fact]
        public void EnterpreterAndEmptyInterpreterElfsShouldNotBeEqual()
        {
            //Arrange
            ElfFormatSummary file1 = new ElfFormatSummary("arm", 32, FormatApi.Endianness.LittleEndian, "/lib/ld-linux.so.2");
            ElfFormatSummary file2 = new ElfFormatSummary("arm", 32, FormatApi.Endianness.LittleEndian, string.Empty);

            //Act
            //Assert
            Assert.NotEqual(file1, file2);
            Assert.NotEqual(file2, file1);
            Assert.NotEqual(file1.GetHashCode(), file2.GetHashCode());
        }
    }
}
