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

            AppConfiguration parameters = new AppConfiguration();
            CommandLineParser parser = new CommandLineParser();
            parser.Configure(parameters);

            //Act            
            parser.Parse(args);

            //Assert
            Assert.False(parser.HelpRequested(args));

            Assert.Single(parameters.Paths);
            Assert.Equal("C:\\Windows", parameters.Paths[0]);
            Assert.True(parameters.Recursive);
            Assert.Null(parameters.Threads);
            Assert.False(parameters.Verbose);
        }

        [Fact]
        public void SeveralDirectoriesArgsShouldBeParsed()
        {
            //Arrange
            string[] args = new string[] { "C:\\Windows", "C:\\Users", "C:\\test.txt" };

            AppConfiguration parameters = new AppConfiguration();
            CommandLineParser parser = new CommandLineParser();
            parser.Configure(parameters);

            //Act            
            parser.Parse(args);

            //Assert
            Assert.False(parser.HelpRequested(args));
            Assert.NotNull(parameters);

            Assert.Equal(3, parameters.Paths.Count());

            Assert.Equal("C:\\Windows", parameters.Paths[0]);
            Assert.Equal("C:\\Users", parameters.Paths[1]);
            Assert.Equal("C:\\test.txt", parameters.Paths[2]);
            Assert.True(parameters.Recursive);
            Assert.Null(parameters.Threads);
            Assert.False(parameters.Verbose);
        }

        [Fact]
        public void SeveralDirectoriesAndThreadsArgsShouldBeParsed()
        {
            //Arrange
            string[] args = new string[] { "C:\\Windows", "C:\\Users", "C:\\test.txt", "--threads", "6" };

            AppConfiguration parameters = new AppConfiguration();
            CommandLineParser parser = new CommandLineParser();
            parser.Configure(parameters);

            //Act            
            parser.Parse(args);

            //Assert
            Assert.False(parser.HelpRequested(args));
            Assert.NotNull(parameters);

            Assert.Equal(3, parameters.Paths.Count());

            Assert.Equal("C:\\Windows", parameters.Paths[0]);
            Assert.Equal("C:\\Users", parameters.Paths[1]);
            Assert.Equal("C:\\test.txt", parameters.Paths[2]);
            Assert.True(parameters.Recursive);
            Assert.NotNull(parameters.Threads);
            Assert.Equal(6, parameters.Threads);
            Assert.False(parameters.Verbose);
        }

        [Fact]
        public void SeveralDirectoriesAndThreadsLongArgsShouldBeParsed()
        {
            //Arrange
            string[] args = new string[] { "C:\\Windows", "C:\\Users", "C:\\test.txt", "--threads", "6" };

            AppConfiguration parameters = new AppConfiguration();
            CommandLineParser parser = new CommandLineParser();
            parser.Configure(parameters);

            //Act            
            parser.Parse(args);

            //Assert
            Assert.False(parser.HelpRequested(args));
            Assert.NotNull(parameters);

            Assert.Equal(3, parameters.Paths.Count());

            Assert.Equal("C:\\Windows", parameters.Paths[0]);
            Assert.Equal("C:\\Users", parameters.Paths[1]);
            Assert.Equal("C:\\test.txt", parameters.Paths[2]);
            Assert.True(parameters.Recursive);
            Assert.NotNull(parameters.Threads);
            Assert.Equal(6, parameters.Threads);
            Assert.False(parameters.Verbose);
        }

        [Fact]
        public void OneDirectoryThreadsLongNoRecursionArgsShouldBeParsed()
        {
            //Arrange
            string[] args = new string[] { "C:\\Windows", "--threads", "6", "--no-recursion" };

            AppConfiguration parameters = new AppConfiguration();
            CommandLineParser parser = new CommandLineParser();
            parser.Configure(parameters);

            //Act            
            parser.Parse(args);

            //Assert
            Assert.False(parser.HelpRequested(args));
            Assert.NotNull(parameters);

            Assert.Single(parameters.Paths);
            Assert.Equal("C:\\Windows", parameters.Paths[0]);
            Assert.False(parameters.Recursive);
            Assert.NotNull(parameters.Threads);
            Assert.Equal(6, parameters.Threads);
            Assert.False(parameters.Verbose);
        }

        [Fact]
        public void OneDirectoryThreadsLongNoRecursionVerboseArgsShouldBeParsed()
        {
            //Arrange
            string[] args = new string[] { "C:\\Windows", "--threads", "6", "--no-recursion", "--verbose" };

            AppConfiguration parameters = new AppConfiguration();
            CommandLineParser parser = new CommandLineParser();
            parser.Configure(parameters);

            //Act            
            parser.Parse(args);

            //Assert
            Assert.False(parser.HelpRequested(args));
            Assert.NotNull(parameters);

            Assert.Single(parameters.Paths);
            Assert.Equal("C:\\Windows", parameters.Paths[0]);
            Assert.False(parameters.Recursive);
            Assert.NotNull(parameters.Threads);
            Assert.Equal(6, parameters.Threads);
            Assert.True(parameters.Verbose);
        }

        [Fact]
        public void HelpArgShouldBeParsed()
        {
            //Arrange
            string[] args = new string[] { "C:\\Windows", "--help" };

            CommandLineParser parser = new CommandLineParser();

            //Act
            //Assert
            Assert.True(parser.HelpRequested(args));
        }

        [Fact]
        public void UnknownParameterShouldThrowException()
        {
            //Arrange
            string[] args = new string[] { "--wft" };

            CommandLineParser parser = new CommandLineParser();

            //Act
            //Assert
            Assert.Throws<ArgumentException>(() => parser.Parse(args));
        }
    }
}
