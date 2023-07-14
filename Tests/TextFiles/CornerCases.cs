using FormatApi;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextFilesFormat;

namespace Tests.TextFiles
{
    public class CornerCases
    {
        [Fact]
        public async Task OneByteAsciiFileShouldBeDetected()
        {
            //Arrange
            string text = "T";

            //Act
            FormatSummary? format = await TextTestsHelper.EncodeAndDetectFull(text, Encoding.ASCII);

            //Assert
            Assert.NotNull(format);
            TextTestsHelper.CheckFormat(DetectableEncoding.ASCII, false, format);
        }

        [Fact]
        public async Task EmptyFileShouldNotBeRecognized()
        {
            //Arrange
            //Act
            FormatSummary? format = await TextTestsHelper.EncodeAndDetectFull(Array.Empty<byte>(), Array.Empty<byte>());

            //Assert
            Assert.Null(format);
        }

        [Fact]
        public async Task StreamReadExceptionShouldBeRethrown()
        {
            TextFilesDetector detector = new TextFilesDetector();

            var mock = new Mock<Stream>();
            mock.Setup(s => s.Length).Throws<IOException>();

            await Assert.ThrowsAsync<IOException>(async () => await detector.ReadFormat(mock.Object, CancellationToken.None));
        }

        [Fact]
        public async Task LengthLimitShouldIgnoreFileEnding()
        {
            //Arrange
            string text = "Hello, world! Привет!";

            //Act
            FormatSummary? format = await TextTestsHelper.EncodeAndDetectFull(text, TextTestsHelper.Windows1251, lengthLimit: 8);

            //Assert
            Assert.NotNull(format);

            //Shuld be detected as ASCII because first 8 bytes are in ASCII range
            TextTestsHelper.CheckFormat(DetectableEncoding.ASCII, false, format);

            //Act2 (Checking that without limit encoding is detected properly)
            FormatSummary? formatFullLength = await TextTestsHelper.EncodeAndDetectFull(text, TextTestsHelper.Windows1251);

            //Assert2
            Assert.NotNull(formatFullLength);
            TextTestsHelper.CheckFormat(DetectableEncoding.Windows125x, false, formatFullLength);
        }
    }
}
