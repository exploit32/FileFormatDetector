using ElfFormat;
using FileFormatDetector.Core;
using FormatApi;
using MachOFormat;
using PEFormat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextFilesFormat;
using XmlFormat;

namespace Tests
{
    public class FormatDetectorTests
    {
        [Fact]
        public async Task DetectPEFile()
        {
            //Arrange
            //Act
            var formats = await CreateDetectorAndScan("pe-Windows-x64-cmd");

            //Assert
            Assert.Single(formats);
            var format = formats.Single();

            Assert.IsType<PEFormatSummary>(format);
        }

        private async Task<IEnumerable<RecognizedFile>> CreateDetectorAndScan(string file)
        {
            FormatDetectorConfiguration configuration = new FormatDetectorConfiguration()
            {
                Paths = new string[] { file },
                Threads = 1,
            };

            IBinaryFormatDetector[] binaryFormats = new IBinaryFormatDetector[] { new PEFormatDetector(), new ElfFormatDetector(), new MachOFormatDetector() };
            ITextFormatDetector[] textFormats = new ITextFormatDetector[] { new TextFilesDetector() };
            ITextBasedFormatDetector[] textBasedFormatDetectors = new ITextBasedFormatDetector[] { new XmlFormatDetector() };

            FormatDetector detector = new FormatDetector(configuration, binaryFormats, textFormats, textBasedFormatDetectors);

            return await detector.ScanFiles(CancellationToken.None);
        }
    }
}
