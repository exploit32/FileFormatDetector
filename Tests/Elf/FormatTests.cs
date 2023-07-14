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
        public async Task x64LinuxAppShouldBeParsed()
        {
            //Arrange
            var stream = Utilities.GetBinaryStream("elf-Linux-x64-bash");
            ElfFormatDetector detector = new ElfFormatDetector();

            //Act
            var formatSummary = await detector.ReadFormat(stream, CancellationToken.None);

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
        public async Task x86LinuxAppShouldBeParsed()
        {
            //Arrange
            var stream = Utilities.GetBinaryStream("elf-Linux-x86-bash");
            ElfFormatDetector detector = new ElfFormatDetector();

            //Act
            var formatSummary = await detector.ReadFormat(stream, CancellationToken.None);

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
        public async Task ia64HPUXAppShouldBeParsed()
        {
            //Arrange
            var stream = Utilities.GetBinaryStream("elf-HPUX-ia64-bash");
            ElfFormatDetector detector = new ElfFormatDetector();

            //Act
            var formatSummary = await detector.ReadFormat(stream, CancellationToken.None);

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
        public async Task Arm64LinuxAppShouldBeParsed()
        {
            //Arrange
            var stream = Utilities.GetBinaryStream("elf-Linux-ARM64-bash");
            ElfFormatDetector detector = new ElfFormatDetector();

            //Act
            var formatSummary = await detector.ReadFormat(stream, CancellationToken.None);

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
        public async Task PowerPCLinuxAppShouldBeParsed()
        {
            //Arrange
            var stream = Utilities.GetBinaryStream("elf-Linux-PowerPC-bash");
            ElfFormatDetector detector = new ElfFormatDetector();

            //Act
            var formatSummary = await detector.ReadFormat(stream, CancellationToken.None);

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
        public async Task s390LinuxAppShouldBeParsed()
        {
            //Arrange
            var stream = Utilities.GetBinaryStream("elf-Linux-s390-bash");
            ElfFormatDetector detector = new ElfFormatDetector();

            //Act
            var formatSummary = await detector.ReadFormat(stream, CancellationToken.None);

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
        public async Task x86LinuxSharedLibraryShouldBeParsed()
        {
            //Arrange
            var stream = Utilities.GetBinaryStream("elf-Linux-lib-x86.so");
            ElfFormatDetector detector = new ElfFormatDetector();

            //Act
            var formatSummary = await detector.ReadFormat(stream, CancellationToken.None);

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
        public async Task x64LinuxSharedLibraryShouldBeParsed()
        {
            //Arrange
            var stream = Utilities.GetBinaryStream("elf-Linux-lib-x64.so");
            ElfFormatDetector detector = new ElfFormatDetector();

            //Act
            var formatSummary = await detector.ReadFormat(stream, CancellationToken.None);

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
        public async Task SparcSolarisAppShouldBeParsed()
        {
            //Arrange
            var stream = Utilities.GetBinaryStream("elf-solaris-sparc-ls");
            ElfFormatDetector detector = new ElfFormatDetector();

            //Act
            var formatSummary = await detector.ReadFormat(stream, CancellationToken.None);

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
        public async Task SparcV8LinuxAppShouldBeParsed()
        {
            //Arrange
            var stream = Utilities.GetBinaryStream("elf-Linux-SparcV8-bash");
            ElfFormatDetector detector = new ElfFormatDetector();

            //Act
            var formatSummary = await detector.ReadFormat(stream, CancellationToken.None);

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
        public async Task SuperH4LinuxAppShouldBeParsed()
        {
            //Arrange
            var stream = Utilities.GetBinaryStream("elf-Linux-SuperH4-bash");
            ElfFormatDetector detector = new ElfFormatDetector();

            //Act
            var formatSummary = await detector.ReadFormat(stream, CancellationToken.None);

            //Assert
            Assert.NotNull(formatSummary);
            Assert.IsType<ElfFormatSummary>(formatSummary);

            var elfFormatSummary = (ElfFormatSummary)formatSummary;

            Assert.Equal(Machine.SuperH.ToString(), elfFormatSummary.Architecture);
            Assert.Equal(FormatApi.Endianness.LittleEndian, elfFormatSummary.Endianness);
            Assert.Equal(32, elfFormatSummary.Bits);
            Assert.Equal("/lib/ld-linux.so.2", elfFormatSummary.Interpreter);
        }

        [Fact]
        public async Task MalformedElfShouldThrowException()
        {
            //Arrange
            byte[] file = new byte[] { 0x7F, 0x45, 0x4C, 0x46, 0x02, 0x01, 0x01, 0x00, 0x00, 0x00 };

            ElfFormatDetector detector = new ElfFormatDetector();

            using (var stream = new MemoryStream(file))
            {
                //Act
                //Assert
                await Assert.ThrowsAsync<FormatException>(async () => await detector.ReadFormat(stream, CancellationToken.None));
            }

        }
    }
}
