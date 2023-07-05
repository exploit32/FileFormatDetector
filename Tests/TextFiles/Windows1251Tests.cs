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
        public void TestRussianText()
        {
            //Arrange
            string text = "Привет!";

            //Act
            DetectableEncoding? encoding = TextTestsHelper.EncodeAndDetect(text, TextTestsHelper.Windows1251);

            //Assert
            Assert.NotNull(encoding);
            Assert.Equal(DetectableEncoding.Windows125x, encoding);
        }

        [Fact]
        public void TestRussianTextWithEvenTextLength()
        {
            //Arrange
            string text = "При  вет";

            //Act
            DetectableEncoding? encoding = TextTestsHelper.EncodeAndDetect(text, TextTestsHelper.Windows1251);

            //Assert
            Assert.NotNull(encoding);
            Assert.Equal(DetectableEncoding.Windows125x, encoding);
        }

        [Fact]
        public void TestEnglishAndRussianText()
        {
            //Arrange
            string text = "Hello, Hi\nПривет!";

            //Act
            DetectableEncoding? encoding = TextTestsHelper.EncodeAndDetect(text, TextTestsHelper.Windows1251);

            //Assert
            Assert.NotNull(encoding);
            Assert.Equal(DetectableEncoding.Windows125x, encoding);
        }
    }
}
