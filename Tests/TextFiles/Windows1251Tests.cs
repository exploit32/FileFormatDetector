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
        [Fact]
        public async Task TestRussianText()
        {
            //Arrange
            string text = "Привет!";

            //Act
            DetectableEncoding? encoding = await TextTestsHelper.EncodeAndDetect(text, TextTestsHelper.Windows1251);

            //Assert
            Assert.NotNull(encoding);
            Assert.Equal(DetectableEncoding.Windows125x, encoding);
        }

        [Fact]
        public async Task TestRussianTextWithEvenTextLength()
        {
            //Arrange
            string text = "При  вет";

            //Act
            DetectableEncoding? encoding = await TextTestsHelper.EncodeAndDetect(text, TextTestsHelper.Windows1251);

            //Assert
            Assert.NotNull(encoding);
            Assert.Equal(DetectableEncoding.Windows125x, encoding);
        }

        [Fact]
        public async Task TestEnglishAndRussianText()
        {
            //Arrange
            string text = "Hello, Hi\nПривет!";

            //Act
            DetectableEncoding? encoding = await TextTestsHelper.EncodeAndDetect(text, TextTestsHelper.Windows1251);

            //Assert
            Assert.NotNull(encoding);
            Assert.Equal(DetectableEncoding.Windows125x, encoding);
        }
    }
}
