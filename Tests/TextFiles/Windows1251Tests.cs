using FormatApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextFilesFormat;

namespace Tests.TextFiles
{
    public class Windows1251Tests
    {
        [Theory]
        [InlineData("Привет!")]
        [InlineData("При  вет")]
        [InlineData("Hello, Hi\nПривет!")]
        public async Task Windows1251TextShouldBeDetected(string text)
        {
            //Arrange
            //Act
            FormatSummary? format = await TextTestsHelper.EncodeAndDetectFull(text, TextTestsHelper.Windows1251);

            //Assert
            Assert.NotNull(format);
            TextTestsHelper.CheckFormat(DetectableEncoding.Windows125x, false, format);
        }
    }
}
