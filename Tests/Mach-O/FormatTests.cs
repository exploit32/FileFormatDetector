using MachOFormat;
using MachOFormat.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Mach_O
{
    public class FormatTests
    {
        [Fact]
        public void x86_OSX()
        {
            //Arrange
            var stream = Utilities.GetBinaryStream("MachO-OSX-x86-ls");
            MachOFormatDetector detector = new MachOFormatDetector();

            //Act
            var formatSummary = detector.ReadFormat(stream);

            //Assert
            Assert.NotNull(formatSummary);
            Assert.IsType<MachOFormatSummary>(formatSummary);

            var machOFormatSummary = (MachOFormatSummary)formatSummary;

            Assert.Equal(CpuType.X86.ToString(), machOFormatSummary.Architecture);
            Assert.Equal(FormatApi.Endianness.LittleEndian, machOFormatSummary.Endianness);
            Assert.Equal(32, machOFormatSummary.Bits);
            Assert.False(machOFormatSummary.IsFat);
            Assert.True(machOFormatSummary.HasSignature);
        }

        [Fact]
        public void x64_OSX()
        {
            //Arrange
            var stream = Utilities.GetBinaryStream("MachO-OSX-x64-ls");
            MachOFormatDetector detector = new MachOFormatDetector();

            //Act
            var formatSummary = detector.ReadFormat(stream);

            //Assert
            Assert.NotNull(formatSummary);
            Assert.IsType<MachOFormatSummary>(formatSummary);

            var machOFormatSummary = (MachOFormatSummary)formatSummary;

            Assert.Equal(CpuType.X86_64.ToString(), machOFormatSummary.Architecture);
            Assert.Equal(FormatApi.Endianness.LittleEndian, machOFormatSummary.Endianness);
            Assert.Equal(64, machOFormatSummary.Bits);
            Assert.False(machOFormatSummary.IsFat);
            Assert.True(machOFormatSummary.HasSignature);
        }

        [Fact]
        public void PowerPC_OSX()
        {
            //Arrange
            var stream = Utilities.GetBinaryStream("MachO-OSX-ppc-openssl-1.0.1h");
            MachOFormatDetector detector = new MachOFormatDetector();

            //Act
            var formatSummary = detector.ReadFormat(stream);

            //Assert
            Assert.NotNull(formatSummary);
            Assert.IsType<MachOFormatSummary>(formatSummary);

            var machOFormatSummary = (MachOFormatSummary)formatSummary;

            Assert.Equal(CpuType.POWERPC.ToString(), machOFormatSummary.Architecture);
            Assert.Equal(FormatApi.Endianness.BigEndian, machOFormatSummary.Endianness);
            Assert.Equal(32, machOFormatSummary.Bits);
            Assert.False(machOFormatSummary.IsFat);
            Assert.False(machOFormatSummary.HasSignature);
        }

        [Fact]
        public void Fat_PowerPC_and_x86_OSX()
        {
            //Arrange
            var stream = Utilities.GetBinaryStream("MachO-OSX-ppc-and-i386-bash");
            MachOFormatDetector detector = new MachOFormatDetector();

            //Act
            var formatSummary = detector.ReadFormat(stream);

            //Assert
            Assert.NotNull(formatSummary);
            Assert.IsType<MachOFormatSummary>(formatSummary);

            var machOFormatSummary = (MachOFormatSummary)formatSummary;

            Assert.Equal(String.Empty, machOFormatSummary.Architecture);
            Assert.Equal(FormatApi.Endianness.BigEndian, machOFormatSummary.Endianness);
            Assert.Equal(32, machOFormatSummary.Bits);

            Assert.True(machOFormatSummary.IsFat);
            Assert.False(machOFormatSummary.HasSignature);

            Assert.Equal(2, machOFormatSummary.InnerApps.Length);

            var app1 = machOFormatSummary.InnerApps[0];

            Assert.Equal(CpuType.X86.ToString(), app1.Architecture);
            Assert.Equal(FormatApi.Endianness.LittleEndian, app1.Endianness);
            Assert.Equal(32, app1.Bits);
            Assert.False(app1.IsFat);
            Assert.True(app1.HasSignature);
            

            var app2 = machOFormatSummary.InnerApps[1];

            Assert.Equal(CpuType.POWERPC.ToString(), app2.Architecture);
            Assert.Equal(FormatApi.Endianness.BigEndian, app2.Endianness);
            Assert.Equal(32, app2.Bits);
            Assert.False(app2.IsFat);
            Assert.True(app2.HasSignature);
        }
    }
}
