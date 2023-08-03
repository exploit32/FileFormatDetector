using MachOFormat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.MachO
{
    public class ComparisonTests
    {
        [Fact]
        public void RegularMachOShouldBeEqual()
        {
            //Arrange
            MachOFormatSummary file1 = new MachOFormatSummary("x86", 32, FormatApi.Endianness.LittleEndian, true, false, Array.Empty<MachOFormatSummary>());
            MachOFormatSummary file2 = new MachOFormatSummary("x86", 32, FormatApi.Endianness.LittleEndian, true, false, Array.Empty<MachOFormatSummary>());

            //Act
            //Assert
            Assert.Equal(file1, file2);
            Assert.Equal(file2, file1);
            Assert.Equal(file1.GetHashCode(), file2.GetHashCode());
        }

        [Fact]
        public void FatMachOWithSameStructureShouldBeEqual()
        {
            //Arrange
            MachOFormatSummary file1 = new MachOFormatSummary("x86", 32, FormatApi.Endianness.LittleEndian, true, true,
                new MachOFormatSummary[]
                {
                    new MachOFormatSummary("arm", 64, FormatApi.Endianness.BigEndian, true, false, Array.Empty<MachOFormatSummary>()),
                    new MachOFormatSummary("arm", 32, FormatApi.Endianness.LittleEndian, true, false, Array.Empty<MachOFormatSummary>()),
                });

            MachOFormatSummary file2 = new MachOFormatSummary("x86", 32, FormatApi.Endianness.LittleEndian, true, true,
                new MachOFormatSummary[]
                {
                    new MachOFormatSummary("arm", 64, FormatApi.Endianness.BigEndian, true, false, Array.Empty<MachOFormatSummary>()),
                    new MachOFormatSummary("arm", 32, FormatApi.Endianness.LittleEndian, true, false, Array.Empty<MachOFormatSummary>()),
                });

            //Act
            //Assert
            Assert.Equal(file1, file2);
            Assert.Equal(file2, file1);
            Assert.Equal(file1.GetHashCode(), file2.GetHashCode());
        }

        [Fact]
        public void FatMachOWithMultipleInnerAppsShouldBeEqual()
        {
            //Arrange
            MachOFormatSummary file1 = new MachOFormatSummary("x86", 32, FormatApi.Endianness.LittleEndian, true, true,
                new MachOFormatSummary[]
                {
                    new MachOFormatSummary("arm", 64, FormatApi.Endianness.BigEndian, true, false, Array.Empty<MachOFormatSummary>()),
                    new MachOFormatSummary("arm", 32, FormatApi.Endianness.LittleEndian, true, false, Array.Empty<MachOFormatSummary>()),
                    new MachOFormatSummary("x64", 64, FormatApi.Endianness.LittleEndian, true, false, Array.Empty<MachOFormatSummary>()),
                });

            MachOFormatSummary file2 = new MachOFormatSummary("x86", 32, FormatApi.Endianness.LittleEndian, true, true,
                new MachOFormatSummary[]
                {
                    new MachOFormatSummary("arm", 64, FormatApi.Endianness.BigEndian, true, false, Array.Empty<MachOFormatSummary>()),
                    new MachOFormatSummary("arm", 32, FormatApi.Endianness.LittleEndian, true, false, Array.Empty<MachOFormatSummary>()),
                    new MachOFormatSummary("x64", 64, FormatApi.Endianness.LittleEndian, true, false, Array.Empty<MachOFormatSummary>()),
                });

            //Act
            //Assert
            Assert.Equal(file1, file2);
            Assert.Equal(file2, file1);
            Assert.Equal(file1.GetHashCode(), file2.GetHashCode());
        }

        [Fact]
        public void FatMachOWithDifferentInnerAppsOrderShouldNotBeEqual()
        {
            //Arrange
            MachOFormatSummary file1 = new MachOFormatSummary("x86", 32, FormatApi.Endianness.LittleEndian, true, true,
                new MachOFormatSummary[]
                {
                    new MachOFormatSummary("arm", 64, FormatApi.Endianness.BigEndian, true, false, Array.Empty<MachOFormatSummary>()),
                    new MachOFormatSummary("arm", 32, FormatApi.Endianness.LittleEndian, true, false, Array.Empty<MachOFormatSummary>()),
                    new MachOFormatSummary("x64", 64, FormatApi.Endianness.LittleEndian, true, false, Array.Empty<MachOFormatSummary>()),
                });

            MachOFormatSummary file2 = new MachOFormatSummary("x86", 32, FormatApi.Endianness.LittleEndian, true, true,
                new MachOFormatSummary[]
                {
                    new MachOFormatSummary("x64", 64, FormatApi.Endianness.LittleEndian, true, false, Array.Empty<MachOFormatSummary>()),
                    new MachOFormatSummary("arm", 64, FormatApi.Endianness.BigEndian, true, false, Array.Empty<MachOFormatSummary>()),
                    new MachOFormatSummary("arm", 32, FormatApi.Endianness.LittleEndian, true, false, Array.Empty<MachOFormatSummary>()),                    
                });

            //Act
            //Assert
            Assert.NotEqual(file1, file2);
            Assert.NotEqual(file2, file1);
            Assert.NotEqual(file1.GetHashCode(), file2.GetHashCode());
        }

        [Fact]
        public void FatAndRegularMachOShouldNotBeEqual()
        {
            //Arrange
            MachOFormatSummary file1 = new MachOFormatSummary("x86", 32, FormatApi.Endianness.LittleEndian, true, true,
                new MachOFormatSummary[]
                {
                    new MachOFormatSummary("arm", 64, FormatApi.Endianness.BigEndian, true, false, Array.Empty<MachOFormatSummary>()),
                    new MachOFormatSummary("arm", 32, FormatApi.Endianness.LittleEndian, true, false, Array.Empty<MachOFormatSummary>()),
                    new MachOFormatSummary("x64", 64, FormatApi.Endianness.LittleEndian, true, false, Array.Empty<MachOFormatSummary>()),
                });

            MachOFormatSummary file2 = new MachOFormatSummary("x86", 32, FormatApi.Endianness.LittleEndian, true, false, Array.Empty<MachOFormatSummary>());

            //Act
            //Assert
            Assert.NotEqual(file1, file2);
            Assert.NotEqual(file2, file1);
            Assert.NotEqual(file1.GetHashCode(), file2.GetHashCode());
        }

        [Fact]
        public void SignedAndUnsignedMachOShouldNotBeEqual()
        {
            //Arrange
            MachOFormatSummary file1 = new MachOFormatSummary("x86", 32, FormatApi.Endianness.LittleEndian, false, false, Array.Empty<MachOFormatSummary>());
            MachOFormatSummary file2 = new MachOFormatSummary("x86", 32, FormatApi.Endianness.LittleEndian, true, false, Array.Empty<MachOFormatSummary>());

            //Act
            //Assert
            Assert.NotEqual(file1, file2);
            Assert.NotEqual(file2, file1);
            Assert.NotEqual(file1.GetHashCode(), file2.GetHashCode());
        }

        [Fact]
        public void LittleBigEndianMachOShouldNotBeEqual()
        {
            //Arrange
            MachOFormatSummary file1 = new MachOFormatSummary("x86", 32, FormatApi.Endianness.LittleEndian, false, false, Array.Empty<MachOFormatSummary>());
            MachOFormatSummary file2 = new MachOFormatSummary("x86", 32, FormatApi.Endianness.BigEndian, false, false, Array.Empty<MachOFormatSummary>());

            //Act
            //Assert
            Assert.NotEqual(file1, file2);
            Assert.NotEqual(file2, file1);
            Assert.NotEqual(file1.GetHashCode(), file2.GetHashCode());
        }

        [Fact]
        public void x86AndArmMachOShouldNotBeEqual()
        {
            //Arrange
            MachOFormatSummary file1 = new MachOFormatSummary("x86", 32, FormatApi.Endianness.LittleEndian, false, false, Array.Empty<MachOFormatSummary>());
            MachOFormatSummary file2 = new MachOFormatSummary("arm", 32, FormatApi.Endianness.LittleEndian, false, false, Array.Empty<MachOFormatSummary>());

            //Act
            //Assert
            Assert.NotEqual(file1, file2);
            Assert.NotEqual(file2, file1);
            Assert.NotEqual(file1.GetHashCode(), file2.GetHashCode());
        }

        [Fact]
        public void Bits32And64MachOShouldNotBeEqual()
        {
            //Arrange
            MachOFormatSummary file1 = new MachOFormatSummary("x86", 64, FormatApi.Endianness.LittleEndian, false, false, Array.Empty<MachOFormatSummary>());
            MachOFormatSummary file2 = new MachOFormatSummary("x86", 32, FormatApi.Endianness.LittleEndian, false, false, Array.Empty<MachOFormatSummary>());

            //Act
            //Assert
            Assert.NotEqual(file1, file2);
            Assert.NotEqual(file2, file1);
            Assert.NotEqual(file1.GetHashCode(), file2.GetHashCode());
        }
    }
}
