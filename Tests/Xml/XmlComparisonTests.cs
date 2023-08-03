using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XmlFormat;

namespace Tests.Xml
{
    public class XmlComparisonTests
    {
        [Fact]
        public void IdenticalFormatsShouldBeEqual()
        {
            //Arrange
            XmlFormatSummary file1 = new XmlFormatSummary("UTF-8", new FormatApi.TextFormatSummary("UTF-8", "Unicode", 1, true));
            XmlFormatSummary file2 = new XmlFormatSummary("UTF-8", new FormatApi.TextFormatSummary("UTF-8", "Unicode", 1, true));

            //Act
            //Assert
            Assert.Equal(file1.GetHashCode(), file2.GetHashCode());
            Assert.Equal(file1, file2);
        }

        [Fact]
        public void IdenticalFormatsWithoutBOMShouldBeEqual()
        {
            //Arrange
            XmlFormatSummary file1 = new XmlFormatSummary("UTF-8", new FormatApi.TextFormatSummary("UTF-8", "Unicode", 1, false));
            XmlFormatSummary file2 = new XmlFormatSummary("UTF-8", new FormatApi.TextFormatSummary("UTF-8", "Unicode", 1, false));

            //Act
            //Assert
            Assert.Equal(file1.GetHashCode(), file2.GetHashCode());
            Assert.Equal(file1, file2);
        }

        [Fact]
        public void IdenticalFormatsWithoutBOMAndXmlDeclarationShouldBeEqual()
        {
            //Arrange
            XmlFormatSummary file1 = new XmlFormatSummary(String.Empty, new FormatApi.TextFormatSummary("UTF-8", "Unicode", 1, false));
            XmlFormatSummary file2 = new XmlFormatSummary(String.Empty, new FormatApi.TextFormatSummary("UTF-8", "Unicode", 1, false));

            //Act
            //Assert
            Assert.Equal(file1.GetHashCode(), file2.GetHashCode());
            Assert.Equal(file1, file2);
        }

        [Fact]
        public void BomAndNonBomFormatsShouldNotBeEqual()
        {
            //Arrange
            XmlFormatSummary file1 = new XmlFormatSummary("UTF-8", new FormatApi.TextFormatSummary("UTF-8", "Unicode", 1, false));
            XmlFormatSummary file2 = new XmlFormatSummary("UTF-8", new FormatApi.TextFormatSummary("UTF-8", "Unicode", 1, true));

            //Act
            //Assert
            Assert.NotEqual(file1.GetHashCode(), file2.GetHashCode());
            Assert.NotEqual(file1, file2);
        }

        [Fact]
        public void XmlDeclarationPresenceDifferenceShouldNotBeEqual()
        {
            //Arrange
            XmlFormatSummary file1 = new XmlFormatSummary("UTF-8", new FormatApi.TextFormatSummary("UTF-8", "Unicode", 1, true));
            XmlFormatSummary file2 = new XmlFormatSummary(string.Empty, new FormatApi.TextFormatSummary("UTF-8", "Unicode", 1, true));

            //Act
            //Assert
            Assert.NotEqual(file1.GetHashCode(), file2.GetHashCode());
            Assert.NotEqual(file1, file2);
        }

        [Fact]
        public void XmlDeclarationEncodingDifferenceShouldNotBeEqual()
        {
            //Arrange
            XmlFormatSummary file1 = new XmlFormatSummary("UTF-16", new FormatApi.TextFormatSummary("UTF-8", "Unicode", 1, true));
            XmlFormatSummary file2 = new XmlFormatSummary("UTF-8", new FormatApi.TextFormatSummary("UTF-8", "Unicode", 1, true));

            //Act
            //Assert
            Assert.NotEqual(file1.GetHashCode(), file2.GetHashCode());
            Assert.NotEqual(file1, file2);
        }

        [Fact]
        public void EncodingDifferenceShouldNotBeEqual()
        {
            //Arrange
            XmlFormatSummary file1 = new XmlFormatSummary(String.Empty, new FormatApi.TextFormatSummary("UTF-8", "Unicode", 1, true));
            XmlFormatSummary file2 = new XmlFormatSummary(string.Empty, new FormatApi.TextFormatSummary("UTF-16", "Utf-16", 2, true));

            //Act
            //Assert
            Assert.NotEqual(file1.GetHashCode(), file2.GetHashCode());
            Assert.NotEqual(file1, file2);
        }

        [Fact]
        public void XmlDeclarationEncodingWithCaseDifferenceShouldNotBeEqual()
        {
            //Arrange
            XmlFormatSummary file1 = new XmlFormatSummary("utf-8", new FormatApi.TextFormatSummary("UTF-8", "Unicode", 1, false));
            XmlFormatSummary file2 = new XmlFormatSummary("UTF-8", new FormatApi.TextFormatSummary("UTF-8", "Unicode", 1, false));

            //Act
            //Assert
            Assert.NotEqual(file1.GetHashCode(), file2.GetHashCode());
            Assert.NotEqual(file1, file2);
        }
    }
}
