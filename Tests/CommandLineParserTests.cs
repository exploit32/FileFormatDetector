using FileFormatDetector.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public class CommandLineParserTests
    {
        [Fact]
        public void OneDirectoryArgShouldBeParsed()
        {
            //Arrange
            string[] args = new string[] { "C:\\Windows" };

            CommandLineParser parser = new CommandLineParser(args, new Parameter[0]);

            //Act
            var parameters = parser.Parse();

            //Assert
            Assert.False(parser.HelpRequested());
            Assert.NotNull(parameters);

            Assert.NotNull(parameters.DetectorConfiguration);
            Assert.Single(parameters.Paths);
            Assert.Equal("C:\\Windows", parameters.Paths[0]);
            Assert.True(parameters.DetectorConfiguration.Recursive);
            Assert.Null(parameters.DetectorConfiguration.Threads);
            Assert.False(parameters.Verbose);
        }

        [Fact]
        public void SeveralDirectoriesArgsShouldBeParsed()
        {
            //Arrange
            string[] args = new string[] { "C:\\Windows", "C:\\Users", "C:\\test.txt" };

            CommandLineParser parser = new CommandLineParser(args, new Parameter[0]);

            //Act
            var parameters = parser.Parse();

            //Assert
            Assert.False(parser.HelpRequested());
            Assert.NotNull(parameters);

            Assert.NotNull(parameters.DetectorConfiguration);
            Assert.Equal(3, parameters.Paths.Length);

            Assert.Equal("C:\\Windows", parameters.Paths[0]);
            Assert.Equal("C:\\Users", parameters.Paths[1]);
            Assert.Equal("C:\\test.txt", parameters.Paths[2]);
            Assert.True(parameters.DetectorConfiguration.Recursive);
            Assert.Null(parameters.DetectorConfiguration.Threads);
            Assert.False(parameters.Verbose);
        }

        [Fact]
        public void SeveralDirectoriesAndThreadsArgsShouldBeParsed()
        {
            //Arrange
            string[] args = new string[] { "C:\\Windows", "C:\\Users", "C:\\test.txt", "-t", "6" };

            CommandLineParser parser = new CommandLineParser(args, new Parameter[0]);

            //Act
            var parameters = parser.Parse();

            //Assert
            Assert.False(parser.HelpRequested());
            Assert.NotNull(parameters);

            Assert.NotNull(parameters.DetectorConfiguration);
            Assert.Equal(3, parameters.Paths.Length);

            Assert.Equal("C:\\Windows", parameters.Paths[0]);
            Assert.Equal("C:\\Users", parameters.Paths[1]);
            Assert.Equal("C:\\test.txt", parameters.Paths[2]);
            Assert.True(parameters.DetectorConfiguration.Recursive);
            Assert.NotNull(parameters.DetectorConfiguration.Threads);
            Assert.Equal(6, parameters.DetectorConfiguration.Threads);
            Assert.False(parameters.Verbose);
        }

        [Fact]
        public void SeveralDirectoriesAndThreadsLongArgsShouldBeParsed()
        {
            //Arrange
            string[] args = new string[] { "C:\\Windows", "C:\\Users", "C:\\test.txt", "--threads", "6" };

            CommandLineParser parser = new CommandLineParser(args, new Parameter[0]);

            //Act
            var parameters = parser.Parse();

            //Assert
            Assert.False(parser.HelpRequested());
            Assert.NotNull(parameters);

            Assert.NotNull(parameters.DetectorConfiguration);
            Assert.Equal(3, parameters.Paths.Length);

            Assert.Equal("C:\\Windows", parameters.Paths[0]);
            Assert.Equal("C:\\Users", parameters.Paths[1]);
            Assert.Equal("C:\\test.txt", parameters.Paths[2]);
            Assert.True(parameters.DetectorConfiguration.Recursive);
            Assert.NotNull(parameters.DetectorConfiguration.Threads);
            Assert.Equal(6, parameters.DetectorConfiguration.Threads);
            Assert.False(parameters.Verbose);
        }

        [Fact]
        public void OneDirectoryThreadsLongNoRecursionArgsShouldBeParsed()
        {
            //Arrange
            string[] args = new string[] { "C:\\Windows", "--threads", "6", "--no-recursion" };

            CommandLineParser parser = new CommandLineParser(args, new Parameter[0]);

            //Act
            var parameters = parser.Parse();

            //Assert
            Assert.False(parser.HelpRequested());
            Assert.NotNull(parameters);

            Assert.NotNull(parameters.DetectorConfiguration);
            Assert.Single(parameters.Paths);
            Assert.Equal("C:\\Windows", parameters.Paths[0]);
            Assert.False(parameters.DetectorConfiguration.Recursive);
            Assert.NotNull(parameters.DetectorConfiguration.Threads);
            Assert.Equal(6, parameters.DetectorConfiguration.Threads);
            Assert.False(parameters.Verbose);
        }

        [Fact]
        public void OneDirectoryThreadsLongNoRecursionVerboseArgsShouldBeParsed()
        {
            //Arrange
            string[] args = new string[] { "C:\\Windows", "--threads", "6", "-n", "--verbose" };

            CommandLineParser parser = new CommandLineParser(args, new Parameter[0]);

            //Act
            var parameters = parser.Parse();

            //Assert
            Assert.False(parser.HelpRequested());
            Assert.NotNull(parameters);

            Assert.NotNull(parameters.DetectorConfiguration);
            Assert.Single(parameters.Paths);
            Assert.Equal("C:\\Windows", parameters.Paths[0]);
            Assert.False(parameters.DetectorConfiguration.Recursive);
            Assert.NotNull(parameters.DetectorConfiguration.Threads);
            Assert.Equal(6, parameters.DetectorConfiguration.Threads);
            Assert.True(parameters.Verbose);
        }

        [Fact]
        public void HelpArgShouldBeParsed()
        {
            //Arrange
            string[] args = new string[] { "C:\\Windows", "--help" };

            CommandLineParser parser = new CommandLineParser(args, new Parameter[0]);

            //Act
            //Assert
            Assert.True(parser.HelpRequested());
        }

        [Fact]
        public void UnknownParameterShouldThrowException()
        {
            //Arrange
            string[] args = new string[] { "--wft" };

            CommandLineParser parser = new CommandLineParser(args, new Parameter[0]);

            //Act
            //Assert
            Assert.Throws<ArgumentException>(() => parser.Parse());
        }
    }
}
