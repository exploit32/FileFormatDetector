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
        public void OneDirectory()
        {
            //Arrange
            string[] args = new string[] { "C:\\Windows" };

            CommandLineParser parser = new CommandLineParser(args);

            //Act
            var parameters = parser.Parse();

            //Assert
            Assert.False(parser.HelpRequested());
            Assert.NotNull(parameters);

            Assert.NotNull(parameters.DetectorConfiguration);
            Assert.Single(parameters.DetectorConfiguration.Paths);
            Assert.Equal("C:\\Windows", parameters.DetectorConfiguration.Paths[0]);
            Assert.True(parameters.DetectorConfiguration.Recursive);
            Assert.Null(parameters.DetectorConfiguration.Threads);
            Assert.False(parameters.Verbose);
        }

        [Fact]
        public void SeveralDirectories()
        {
            //Arrange
            string[] args = new string[] { "C:\\Windows", "C:\\Users", "C:\\test.txt" };

            CommandLineParser parser = new CommandLineParser(args);

            //Act
            var parameters = parser.Parse();

            //Assert
            Assert.False(parser.HelpRequested());
            Assert.NotNull(parameters);

            Assert.NotNull(parameters.DetectorConfiguration);
            Assert.Equal(3, parameters.DetectorConfiguration.Paths.Length);

            Assert.Equal("C:\\Windows", parameters.DetectorConfiguration.Paths[0]);
            Assert.Equal("C:\\Users", parameters.DetectorConfiguration.Paths[1]);
            Assert.Equal("C:\\test.txt", parameters.DetectorConfiguration.Paths[2]);
            Assert.True(parameters.DetectorConfiguration.Recursive);
            Assert.Null(parameters.DetectorConfiguration.Threads);
            Assert.False(parameters.Verbose);
        }

        [Fact]
        public void SeveralDirectoriesAndThreads()
        {
            //Arrange
            string[] args = new string[] { "C:\\Windows", "C:\\Users", "C:\\test.txt", "-t", "6" };

            CommandLineParser parser = new CommandLineParser(args);

            //Act
            var parameters = parser.Parse();

            //Assert
            Assert.False(parser.HelpRequested());
            Assert.NotNull(parameters);

            Assert.NotNull(parameters.DetectorConfiguration);
            Assert.Equal(3, parameters.DetectorConfiguration.Paths.Length);

            Assert.Equal("C:\\Windows", parameters.DetectorConfiguration.Paths[0]);
            Assert.Equal("C:\\Users", parameters.DetectorConfiguration.Paths[1]);
            Assert.Equal("C:\\test.txt", parameters.DetectorConfiguration.Paths[2]);
            Assert.True(parameters.DetectorConfiguration.Recursive);
            Assert.NotNull(parameters.DetectorConfiguration.Threads);
            Assert.Equal(6, parameters.DetectorConfiguration.Threads);
            Assert.False(parameters.Verbose);
        }

        [Fact]
        public void SeveralDirectoriesAndThreadsLong()
        {
            //Arrange
            string[] args = new string[] { "C:\\Windows", "C:\\Users", "C:\\test.txt", "--threads", "6" };

            CommandLineParser parser = new CommandLineParser(args);

            //Act
            var parameters = parser.Parse();

            //Assert
            Assert.False(parser.HelpRequested());
            Assert.NotNull(parameters);

            Assert.NotNull(parameters.DetectorConfiguration);
            Assert.Equal(3, parameters.DetectorConfiguration.Paths.Length);

            Assert.Equal("C:\\Windows", parameters.DetectorConfiguration.Paths[0]);
            Assert.Equal("C:\\Users", parameters.DetectorConfiguration.Paths[1]);
            Assert.Equal("C:\\test.txt", parameters.DetectorConfiguration.Paths[2]);
            Assert.True(parameters.DetectorConfiguration.Recursive);
            Assert.NotNull(parameters.DetectorConfiguration.Threads);
            Assert.Equal(6, parameters.DetectorConfiguration.Threads);
            Assert.False(parameters.Verbose);
        }

        [Fact]
        public void OneDirectoryThreadsLongNoRecursion()
        {
            //Arrange
            string[] args = new string[] { "C:\\Windows", "--threads", "6", "--no-recursion" };

            CommandLineParser parser = new CommandLineParser(args);

            //Act
            var parameters = parser.Parse();

            //Assert
            Assert.False(parser.HelpRequested());
            Assert.NotNull(parameters);

            Assert.NotNull(parameters.DetectorConfiguration);
            Assert.Single(parameters.DetectorConfiguration.Paths);
            Assert.Equal("C:\\Windows", parameters.DetectorConfiguration.Paths[0]);
            Assert.False(parameters.DetectorConfiguration.Recursive);
            Assert.NotNull(parameters.DetectorConfiguration.Threads);
            Assert.Equal(6, parameters.DetectorConfiguration.Threads);
            Assert.False(parameters.Verbose);
        }

        [Fact]
        public void OneDirectoryThreadsLongNoRecursionVerbose()
        {
            //Arrange
            string[] args = new string[] { "C:\\Windows", "--threads", "6", "-n", "--verbose" };

            CommandLineParser parser = new CommandLineParser(args);

            //Act
            var parameters = parser.Parse();

            //Assert
            Assert.False(parser.HelpRequested());
            Assert.NotNull(parameters);

            Assert.NotNull(parameters.DetectorConfiguration);
            Assert.Single(parameters.DetectorConfiguration.Paths);
            Assert.Equal("C:\\Windows", parameters.DetectorConfiguration.Paths[0]);
            Assert.False(parameters.DetectorConfiguration.Recursive);
            Assert.NotNull(parameters.DetectorConfiguration.Threads);
            Assert.Equal(6, parameters.DetectorConfiguration.Threads);
            Assert.True(parameters.Verbose);
        }

        [Fact]
        public void Help()
        {
            //Arrange
            string[] args = new string[] { "C:\\Windows", "--help" };

            CommandLineParser parser = new CommandLineParser(args);

            //Act
            //Assert
            Assert.True(parser.HelpRequested());
        }
    }
}
