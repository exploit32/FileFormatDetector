using ElfFormat;
using ElfFormat.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Elf
{
    public class FormatTests
    {
        [Fact]
        public void x64_linux()
        {
            //Arrange
            var stream = Utilities.GetBinaryStream("elf-Linux-x64-bash");
            ElfFormatDetector detector = new ElfFormatDetector();

            //Act
            var formatSummary = detector.ReadFormat(stream);

            //Assert
            Assert.NotNull(formatSummary);
            Assert.IsType<ElfFormatSummary>(formatSummary);

            var elfFormatSummary = (ElfFormatSummary)formatSummary;

            Assert.Equal(Machine.AMD64.ToString(), elfFormatSummary.Architecture);
            Assert.Equal(FormatApi.Endianness.LittleEndian, elfFormatSummary.Endianness);
            Assert.Equal(64, elfFormatSummary.Bits);
            Assert.Equal("/lib64/ld-linux-x86-64.so.2", elfFormatSummary.Interpreter);
        }

        [Fact]
        public void x86_Linux()
        {
            //Arrange
            var stream = Utilities.GetBinaryStream("elf-Linux-x86-bash");
            ElfFormatDetector detector = new ElfFormatDetector();

            //Act
            var formatSummary = detector.ReadFormat(stream);

            //Assert
            Assert.NotNull(formatSummary);
            Assert.IsType<ElfFormatSummary>(formatSummary);

            var elfFormatSummary = (ElfFormatSummary)formatSummary;

            Assert.Equal(Machine.Intel386.ToString(), elfFormatSummary.Architecture);
            Assert.Equal(FormatApi.Endianness.LittleEndian, elfFormatSummary.Endianness);
            Assert.Equal(32, elfFormatSummary.Bits);
            Assert.Equal("/lib/ld-linux.so.2", elfFormatSummary.Interpreter);
        }

        [Fact]
        public void ia64_HPUX()
        {
            //Arrange
            var stream = Utilities.GetBinaryStream("elf-HPUX-ia64-bash");
            ElfFormatDetector detector = new ElfFormatDetector();

            //Act
            var formatSummary = detector.ReadFormat(stream);

            //Assert
            Assert.NotNull(formatSummary);
            Assert.IsType<ElfFormatSummary>(formatSummary);

            var elfFormatSummary = (ElfFormatSummary)formatSummary;

            Assert.Equal(Machine.IA64.ToString(), elfFormatSummary.Architecture);
            Assert.Equal(FormatApi.Endianness.BigEndian, elfFormatSummary.Endianness);
            Assert.Equal(32, elfFormatSummary.Bits);
            Assert.Equal("/usr/lib/hpux32/uld.so:/usr/lib/hpux32/dld.so", elfFormatSummary.Interpreter);
        }

        [Fact]
        public void Arm64_Linux()
        {
            //Arrange
            var stream = Utilities.GetBinaryStream("elf-Linux-ARM64-bash");
            ElfFormatDetector detector = new ElfFormatDetector();

            //Act
            var formatSummary = detector.ReadFormat(stream);

            //Assert
            Assert.NotNull(formatSummary);
            Assert.IsType<ElfFormatSummary>(formatSummary);

            var elfFormatSummary = (ElfFormatSummary)formatSummary;

            Assert.Equal(Machine.AArch64.ToString(), elfFormatSummary.Architecture);
            Assert.Equal(FormatApi.Endianness.LittleEndian, elfFormatSummary.Endianness);
            Assert.Equal(64, elfFormatSummary.Bits);
            Assert.Equal("/lib/ld-linux-aarch64.so.1", elfFormatSummary.Interpreter);
        }

        [Fact]
        public void PowerPC_Linux()
        {
            //Arrange
            var stream = Utilities.GetBinaryStream("elf-Linux-PowerPC-bash");
            ElfFormatDetector detector = new ElfFormatDetector();

            //Act
            var formatSummary = detector.ReadFormat(stream);

            //Assert
            Assert.NotNull(formatSummary);
            Assert.IsType<ElfFormatSummary>(formatSummary);

            var elfFormatSummary = (ElfFormatSummary)formatSummary;

            Assert.Equal(Machine.PPC.ToString(), elfFormatSummary.Architecture);
            Assert.Equal(FormatApi.Endianness.BigEndian, elfFormatSummary.Endianness);
            Assert.Equal(32, elfFormatSummary.Bits);
            Assert.Equal("/lib/ld.so.1", elfFormatSummary.Interpreter);
        }

        [Fact]
        public void s390_Linux()
        {
            //Arrange
            var stream = Utilities.GetBinaryStream("elf-Linux-s390-bash");
            ElfFormatDetector detector = new ElfFormatDetector();

            //Act
            var formatSummary = detector.ReadFormat(stream);

            //Assert
            Assert.NotNull(formatSummary);
            Assert.IsType<ElfFormatSummary>(formatSummary);

            var elfFormatSummary = (ElfFormatSummary)formatSummary;

            Assert.Equal(Machine.S390.ToString(), elfFormatSummary.Architecture);
            Assert.Equal(FormatApi.Endianness.BigEndian, elfFormatSummary.Endianness);
            Assert.Equal(64, elfFormatSummary.Bits);
            Assert.Equal("/lib/ld64.so.1", elfFormatSummary.Interpreter);
        }

        [Fact]
        public void x86_Linux_Shared_Library()
        {
            //Arrange
            var stream = Utilities.GetBinaryStream("elf-Linux-lib-x86.so");
            ElfFormatDetector detector = new ElfFormatDetector();

            //Act
            var formatSummary = detector.ReadFormat(stream);

            //Assert
            Assert.NotNull(formatSummary);
            Assert.IsType<ElfFormatSummary>(formatSummary);

            var elfFormatSummary = (ElfFormatSummary)formatSummary;

            Assert.Equal(Machine.Intel386.ToString(), elfFormatSummary.Architecture);
            Assert.Equal(FormatApi.Endianness.LittleEndian, elfFormatSummary.Endianness);
            Assert.Equal(32, elfFormatSummary.Bits);
            Assert.Equal(String.Empty, elfFormatSummary.Interpreter);
        }

        [Fact]
        public void x64_Linux_Shared_Library()
        {
            //Arrange
            var stream = Utilities.GetBinaryStream("elf-Linux-lib-x64.so");
            ElfFormatDetector detector = new ElfFormatDetector();

            //Act
            var formatSummary = detector.ReadFormat(stream);

            //Assert
            Assert.NotNull(formatSummary);
            Assert.IsType<ElfFormatSummary>(formatSummary);

            var elfFormatSummary = (ElfFormatSummary)formatSummary;

            Assert.Equal(Machine.AMD64.ToString(), elfFormatSummary.Architecture);
            Assert.Equal(FormatApi.Endianness.LittleEndian, elfFormatSummary.Endianness);
            Assert.Equal(64, elfFormatSummary.Bits);
            Assert.Equal(String.Empty, elfFormatSummary.Interpreter);
        }

        [Fact]
        public void Sparc_Solaris()
        {
            //Arrange
            var stream = Utilities.GetBinaryStream("elf-solaris-sparc-ls");
            ElfFormatDetector detector = new ElfFormatDetector();

            //Act
            var formatSummary = detector.ReadFormat(stream);

            //Assert
            Assert.NotNull(formatSummary);
            Assert.IsType<ElfFormatSummary>(formatSummary);

            var elfFormatSummary = (ElfFormatSummary)formatSummary;

            Assert.Equal(Machine.SPARC.ToString(), elfFormatSummary.Architecture);
            Assert.Equal(FormatApi.Endianness.BigEndian, elfFormatSummary.Endianness);
            Assert.Equal(32, elfFormatSummary.Bits);
            Assert.Equal("/usr/lib/ld.so.1", elfFormatSummary.Interpreter);
        }

        [Fact]
        public void SparcV8_Linux()
        {
            //Arrange
            var stream = Utilities.GetBinaryStream("elf-Linux-SparcV8-bash");
            ElfFormatDetector detector = new ElfFormatDetector();

            //Act
            var formatSummary = detector.ReadFormat(stream);

            //Assert
            Assert.NotNull(formatSummary);
            Assert.IsType<ElfFormatSummary>(formatSummary);

            var elfFormatSummary = (ElfFormatSummary)formatSummary;

            Assert.Equal(Machine.SPARC32Plus.ToString(), elfFormatSummary.Architecture);
            Assert.Equal(FormatApi.Endianness.BigEndian, elfFormatSummary.Endianness);
            Assert.Equal(32, elfFormatSummary.Bits);
            Assert.Equal("/lib/ld-linux.so.2", elfFormatSummary.Interpreter);
        }

        [Fact]
        public void SuperH4_Linux()
        {
            //Arrange
            var stream = Utilities.GetBinaryStream("elf-Linux-SuperH4-bash");
            ElfFormatDetector detector = new ElfFormatDetector();

            //Act
            var formatSummary = detector.ReadFormat(stream);

            //Assert
            Assert.NotNull(formatSummary);
            Assert.IsType<ElfFormatSummary>(formatSummary);

            var elfFormatSummary = (ElfFormatSummary)formatSummary;

            Assert.Equal(Machine.SuperH.ToString(), elfFormatSummary.Architecture);
            Assert.Equal(FormatApi.Endianness.LittleEndian, elfFormatSummary.Endianness);
            Assert.Equal(32, elfFormatSummary.Bits);
            Assert.Equal("/lib/ld-linux.so.2", elfFormatSummary.Interpreter);
        }
    }
}
