using FormatApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.TextFiles
{
    public class ComparisonTests
    {
        [Fact]
        public void IdenticalTextFilesShouldBeEquals()
        {
            //Arrange
            TextFormatSummary file1 = new TextFormatSummary("UTF-8", "Unicode UTF-8", 1, true);
            TextFormatSummary file2 = new TextFormatSummary("UTF-8", "Unicode UTF-8", 1, true);

            //Act
            //Assert
            Assert.Equal(file1, file2);
            Assert.Equal(file2, file1);
            Assert.Equal(file1.GetHashCode(), file2.GetHashCode());
        }

        [Fact]
        public void IdenticalTextFilesShouldBeEquals2()
        {
            //Arrange
            TextFormatSummary file1 = new TextFormatSummary("Windows-1251", "Windows 1251 encoding", 1251, false);
            TextFormatSummary file2 = new TextFormatSummary("Windows-1251", "Windows 1251 encoding", 1251, false);

            //Act
            //Assert
            Assert.Equal(file1, file2);
            Assert.Equal(file2, file1);
            Assert.Equal(file1.GetHashCode(), file2.GetHashCode());
        }

        [Fact]
        public void DifferentEncodingNameTextFilesShouldNotBeEquals()
        {
            //Arrange
            TextFormatSummary file1 = new TextFormatSummary("Windows-1251", "Windows 1251 encoding", 1251, false);
            TextFormatSummary file2 = new TextFormatSummary("UTF-8", "Windows 1251 encoding", 1251, false);

            //Act
            //Assert
            Assert.NotEqual(file1, file2);
            Assert.NotEqual(file2, file1);
            Assert.NotEqual(file1.GetHashCode(), file2.GetHashCode());
        }

        [Fact]
        public void DifferentEncodingFullNameTextFilesShouldNotBeEquals()
        {
            //Arrange
            TextFormatSummary file1 = new TextFormatSummary("UTF-8", "Unicode UTF-8", 1, false);
            TextFormatSummary file2 = new TextFormatSummary("UTF-8", "Unicode UTF 8", 1, false);

            //Act
            //Assert
            Assert.NotEqual(file1, file2);
            Assert.NotEqual(file2, file1);
            Assert.NotEqual(file1.GetHashCode(), file2.GetHashCode());
        }

        [Fact]
        public void DifferentCodePageTextFilesShouldNotBeEquals()
        {
            //Arrange
            TextFormatSummary file1 = new TextFormatSummary("UTF-8", "Unicode UTF-8", 1, false);
            TextFormatSummary file2 = new TextFormatSummary("UTF-8", "Unicode UTF-8", 2, false);

            //Act
            //Assert
            Assert.NotEqual(file1, file2);
            Assert.NotEqual(file2, file1);
            Assert.NotEqual(file1.GetHashCode(), file2.GetHashCode());
        }

        [Fact]
        public void BomAndNonBomTextFilesShouldNotBeEquals()
        {
            //Arrange
            TextFormatSummary file1 = new TextFormatSummary("UTF-8", "Unicode UTF-8", 1, false);
            TextFormatSummary file2 = new TextFormatSummary("UTF-8", "Unicode UTF-8", 1, true);

            //Act
            //Assert
            Assert.NotEqual(file1, file2);
            Assert.NotEqual(file2, file1);
            Assert.NotEqual(file1.GetHashCode(), file2.GetHashCode());
        }
    }
}
