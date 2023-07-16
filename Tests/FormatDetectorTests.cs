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
    public class FormatDetectorTests
    {
        public string BinarySamplesDirectory => Path.Combine(Directory.GetCurrentDirectory(), "Samples", "binary-samples");
        public string TextSamplesDirectory => Path.Combine(Directory.GetCurrentDirectory(), "Samples", "Text");
        public string XmlSamplesDirectory => Path.Combine(Directory.GetCurrentDirectory(), "Samples", "Xml");
        public string UnknownSamplesDirectory => Path.Combine(Directory.GetCurrentDirectory(), "Samples", "Unknown");

        [Fact]
        public async Task BinaryFilesShouldBeDetected()
        {
            //Arrange
            //Act
            var formats = (await CreateDetectorAndScan(new string[] { BinarySamplesDirectory })).ToArray();

            //Assert
            Assert.Equal(5, formats.Count(f => f.FormatSummary is PEFormatSummary));
            Assert.Equal(22, formats.Count(f => f.FormatSummary is ElfFormatSummary));
            Assert.Equal(8, formats.Count(f => f.FormatSummary is MachOFormatSummary));
        }

        [Fact]
        public async Task TextFilesWithBomShouldBeDetected()
        {
            //Arrange
            //Act
            var formats = (await CreateDetectorAndScan(new string[] { Path.Combine(TextSamplesDirectory, "bom") })).ToArray();

            //Assert
            Assert.Equal(10, formats.Count(f => f.FormatSummary is TextFormatSummary));
        }

        [Fact]
        public async Task TextFilesWithoutBomShouldBeDetected()
        {
            //Arrange
            //Act
            var formats = (await CreateDetectorAndScan(new string[] { Path.Combine(TextSamplesDirectory, "no-bom") })).ToArray();

            //Assert
            Assert.Equal(6, formats.Count(f => f.FormatSummary is TextFormatSummary));
        }

        [Fact]
        public async Task XmlFilesShouldBeDetected()
        {
            //Arrange
            //Act
            var formats = (await CreateDetectorAndScan(new string[] { XmlSamplesDirectory })).ToArray();

            //Assert
            Assert.Equal(8, formats.Count(f => f.FormatSummary is XmlFormatSummary));
        }

        [Fact]
        public async Task UnknownFilesShouldBeDetectedAsUnknown()
        {
            //Arrange
            //Act
            var formats = (await CreateDetectorAndScan(new string[] { UnknownSamplesDirectory })).ToArray();

            //Assert
            Assert.All(formats, f => Assert.True(f.IsUnknown));
        }

        private async Task<IEnumerable<FileRecognitionResult>> CreateDetectorAndScan(string[] paths)
        {
            FormatDetectorConfiguration configuration = new FormatDetectorConfiguration();

            IFormatDetector[] binaryFormats = new IFormatDetector[] { new PEFormatDetector(), new ElfFormatDetector(), new MachOFormatDetector(), new TextFilesDetector() };
            ITextBasedFormatDetector[] textBasedFormatDetectors = new ITextBasedFormatDetector[] { new XmlFormatDetector() };

            FormatDetector detector = new FormatDetector(configuration, binaryFormats, textBasedFormatDetectors);

            return await detector.ScanFiles(paths, CancellationToken.None);
        }
    }
}
