using ElfFormat;
using FileFormatDetector.Core;
using FormatApi;
using MachOFormat;
using PEFormat;
using PEFormat.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TextFilesFormat;
using XmlFormat;

namespace Tests
{
    public class FormatDetectorTests : IDisposable
    {
        public string TestsTempDirectory => Path.Combine(Path.GetTempPath(), nameof(FormatDetectorTests));

        public string PeSubdir => Path.Combine(TestsTempDirectory, "pe");
        public string ElfSubdir => Path.Combine(TestsTempDirectory, "elf");
        public string MachOSubdir => Path.Combine(TestsTempDirectory, "mach-o");

        public FormatDetectorTests()
        {
            Directory.CreateDirectory(TestsTempDirectory);

            Directory.CreateDirectory(PeSubdir);
            WriteFile("pe-Windows-x64-cmd", PeSubdir);
            WriteFile("pe-Windows-x86-cmd", PeSubdir);
            WriteFile("pe-Windows-ARMv7-Thumb2LE-HelloWorld", PeSubdir);
            WriteFile("pe-mingw32-strip.exe", PeSubdir);

            Directory.CreateDirectory(ElfSubdir);
            WriteFile("elf-Linux-ARM64-bash", ElfSubdir);
            WriteFile("elf-Linux-s390-bash", ElfSubdir);
            WriteFile("elf-solaris-sparc-ls", ElfSubdir);
            WriteFile("elf-Linux-ia64-bash", ElfSubdir);

            Directory.CreateDirectory(MachOSubdir);
            WriteFile("MachO-iOS-armv7-armv7s-arm64-Helloworld", MachOSubdir);
            WriteFile("MachO-OSX-ppc-and-i386-bash", MachOSubdir);
            WriteFile("MachO-OSX-x64-ls", MachOSubdir);
            WriteFile("libSystem.B.dylib", MachOSubdir);
        }

        [Fact]
        public async Task DetectPEFiles()
        {
            //Arrange
            //Act
            var formats = (await CreateDetectorAndScan(new string[] { PeSubdir })).ToArray();

            //Assert
            Assert.Equal(4, formats.Length);

            foreach (var format in formats)
            {
                Assert.IsType<PEFormatSummary>(format.FormatSummary);
            }
        }

        [Fact]
        public async Task DetectElfFiles()
        {
            //Arrange
            //Act
            var formats = (await CreateDetectorAndScan(new string[] { ElfSubdir })).ToArray();

            //Assert
            Assert.Equal(4, formats.Length);

            foreach (var format in formats)
            {
                Assert.IsType<ElfFormatSummary>(format.FormatSummary);
            }
        }

        [Fact]
        public async Task DetectMachOFiles()
        {
            //Arrange
            //Act
            var formats = (await CreateDetectorAndScan(new string[] { MachOSubdir })).ToArray();

            //Assert
            Assert.Equal(4, formats.Length);

            foreach (var format in formats)
            {
                Assert.IsType<MachOFormatSummary>(format.FormatSummary);
            }
        }

        [Fact]
        public async Task DetectAllFiles()
        {
            //Arrange
            //Act
            var formats = (await CreateDetectorAndScan(new string[] { TestsTempDirectory })).ToArray();

            //Assert
            Assert.Equal(4, formats.Count(f => f.FormatSummary is PEFormatSummary));
            Assert.Equal(4, formats.Count(f => f.FormatSummary is ElfFormatSummary));
            Assert.Equal(4, formats.Count(f => f.FormatSummary is MachOFormatSummary));
        }

        [Fact]
        public async Task DetectTextFilesWithBom()
        {
            //Arrange
            //Act
            var formats = (await CreateDetectorAndScan(new string[] { Path.Combine(Directory.GetCurrentDirectory(), "Samples", "Text", "bom") })).ToArray();

            //Assert
            Assert.Equal(10, formats.Count(f => f.FormatSummary is TextFormatSummary));
        }

        [Fact]
        public async Task DetectTextFilesWithoutBom()
        {
            //Arrange
            //Act
            var formats = (await CreateDetectorAndScan(new string[] { Path.Combine(Directory.GetCurrentDirectory(), "Samples", "Text", "no-bom") })).ToArray();

            //Assert
            Assert.Equal(6, formats.Count(f => f.FormatSummary is TextFormatSummary));
        }

        private async Task<IEnumerable<RecognizedFile>> CreateDetectorAndScan(string[] paths)
        {
            FormatDetectorConfiguration configuration = new FormatDetectorConfiguration()
            {
                Paths = paths,
            };

            IBinaryFormatDetector[] binaryFormats = new IBinaryFormatDetector[] { new PEFormatDetector(), new ElfFormatDetector(), new MachOFormatDetector() };
            ITextFormatDetector[] textFormats = new ITextFormatDetector[] { new TextFilesDetector() };
            ITextBasedFormatDetector[] textBasedFormatDetectors = new ITextBasedFormatDetector[] { new XmlFormatDetector() };

            FormatDetector detector = new FormatDetector(configuration, binaryFormats, textFormats, textBasedFormatDetectors);

            return await detector.ScanFiles(CancellationToken.None);
        }


        private void WriteFile(string name, string dir)
        {
            using (var stream = Utilities.GetBinaryStream(name))
            using (var file = File.OpenWrite(Path.Combine(dir, name)))
            {
                stream.CopyTo(file);
            }
        }

        public void Dispose()
        {
            Directory.Delete(TestsTempDirectory, true);
        }
    }
}
