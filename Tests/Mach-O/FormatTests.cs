﻿using MachOFormat;
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
        public void x86OSX()
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
        public void x64OSX()
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
        public void PowerPcOSX()
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
        public void FatPowerPCandX86OSX()
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

        [Fact]
        public void FatThreeArmsIOS()
        {
            //Arrange
            var stream = Utilities.GetBinaryStream("MachO-iOS-armv7-armv7s-arm64-Helloworld");
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

            Assert.Equal(3, machOFormatSummary.InnerApps.Length);

            var app1 = machOFormatSummary.InnerApps[0];

            Assert.Equal(CpuType.ARM.ToString(), app1.Architecture);
            Assert.Equal(FormatApi.Endianness.LittleEndian, app1.Endianness);
            Assert.Equal(32, app1.Bits);
            Assert.False(app1.IsFat);
            Assert.True(app1.HasSignature);

            var app2 = machOFormatSummary.InnerApps[1];

            Assert.Equal(CpuType.ARM.ToString(), app2.Architecture);
            Assert.Equal(FormatApi.Endianness.LittleEndian, app2.Endianness);
            Assert.Equal(32, app2.Bits);
            Assert.False(app2.IsFat);
            Assert.True(app2.HasSignature);

            var app3 = machOFormatSummary.InnerApps[2];

            Assert.Equal(CpuType.ARM64.ToString(), app3.Architecture);
            Assert.Equal(FormatApi.Endianness.LittleEndian, app3.Endianness);
            Assert.Equal(64, app3.Bits);
            Assert.False(app3.IsFat);
            Assert.True(app3.HasSignature);
        }

        [Fact]
        public void ArmIOS()
        {
            //Arrange
            var stream = Utilities.GetBinaryStream("MachO-iOS-arm1176JZFS-bash");
            MachOFormatDetector detector = new MachOFormatDetector();

            //Act
            var formatSummary = detector.ReadFormat(stream);

            //Assert
            Assert.NotNull(formatSummary);
            Assert.IsType<MachOFormatSummary>(formatSummary);

            var machOFormatSummary = (MachOFormatSummary)formatSummary;

            Assert.Equal(CpuType.ARM.ToString(), machOFormatSummary.Architecture);
            Assert.Equal(FormatApi.Endianness.LittleEndian, machOFormatSummary.Endianness);
            Assert.Equal(32, machOFormatSummary.Bits);
            Assert.False(machOFormatSummary.IsFat);
            Assert.True(machOFormatSummary.HasSignature);
        }

        [Fact]
        public void MalformedMachO()
        {
            //Arrange
            byte[] file = new byte[] { 0xCA, 0xFE, 0xBA, 0xBE, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00, 0x0C, 0x00, 0x00, 0x00, 0x09 };

            MachOFormatDetector detector = new MachOFormatDetector();
            
            using (var stream = new MemoryStream(file))
            {
                //Act
                //Assert
                Assert.Throws<FormatException>(() => detector.ReadFormat(stream));
            }
        }

        [Fact]
        public void MalformedMachOSeekOutOfRange()
        {
            //Arrange
            byte[] onlyFatHeaders = new byte[] { 
                0xCA, 0xFE, 0xBA, 0xBE, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00, 0x0C, 0x00, 0x00, 0x00, 0x09,
                0x00, 0x00, 0x40, 0x00, 0x00, 0x01, 0x66, 0x90, 0x00, 0x00, 0x00, 0x0E, 0x00, 0x00, 0x00, 0x0C,
                0x00, 0x00, 0x00, 0x0B, 0x00, 0x01, 0xC0, 0x00, 0x00, 0x01, 0x66, 0x90, 0x00, 0x00, 0x00, 0x0E,
                0x01, 0x00, 0x00, 0x0C, 0x00, 0x00, 0x00, 0x00, 0x00, 0x03, 0x40, 0x00, 0x00, 0x01, 0x6C, 0x40,
                0x00, 0x00, 0x00, 0x0E
            };

            MachOFormatDetector detector = new MachOFormatDetector();
            
            using (var stream = new MemoryStream(onlyFatHeaders))
            {
                //Act
                //Assert
                Assert.Throws<FormatException>(() => detector.ReadFormat(stream));
            }
        }
    }
}
