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
        public void OneByteFile()
        {
            //Arrange
            string text = "T";

            //Act
            FormatSummary? format = TextTestsHelper.EncodeAndDetectFull(text, Encoding.ASCII);

            //Assert
            Assert.NotNull(format);
            TextTestsHelper.CheckFormat(DetectableEncoding.ASCII, false, format);
        }

        [Fact]
        public void EmptyFileShouldNotBeRecognized()
        {
            //Arrange
            //Act
            FormatSummary? format = TextTestsHelper.EncodeAndDetectFull(Array.Empty<byte>(), Array.Empty<byte>());

            //Assert
            Assert.Null(format);
        }

        [Fact]
        public void ExceptionShouldBeThrown()
        {
            TextFilesDetector detector = new TextFilesDetector();

            var mock = new Mock<Stream>();
            mock.Setup(s => s.Length).Throws<IOException>();

            Assert.ThrowsAsync<IOException>(async () => await detector.ReadFormat(mock.Object, null));
        }
    }
}
