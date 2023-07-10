using FileFormatDetector.Core;


namespace FileFormatDetector.Console
{
    internal class CommandLineParser
    {
        private readonly string[] _args;

        public CommandLineParser(string[] args)
        {
            _args = args;
        }

        public bool HelpRequested()
        {
            return _args.Any(a => a.Equals("-h", StringComparison.InvariantCultureIgnoreCase) || a.Equals("--help", StringComparison.InvariantCultureIgnoreCase));
        }

        public AppConfiguration Parse()
        {
            AppConfiguration appConfiguration = new AppConfiguration();

            List<string> paths = new List<string>();

            int index = 0;

            while(index < _args.Length)
            {
                string current = _args[index++];

                if (current.StartsWith('-'))
                {
                    if (string.Equals(current, "-t", StringComparison.InvariantCultureIgnoreCase) || string.Equals(current, "--threads", StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (index < _args.Length)
                        {
                            string threadsCountStr = _args[index++];

                            if (int.TryParse(threadsCountStr, out var threadsCount))
                                appConfiguration.DetectorConfiguration.Threads = threadsCount;
                            else
                                throw new ArgumentException("Error parsing threads count. Value should be integer.");
                        }
                        else
                        {
                            throw new ArgumentException("Error parsing threads count. Value expected");
                        }
                    }
                    else if (string.Equals(current, "-n", StringComparison.InvariantCultureIgnoreCase) || string.Equals(current, "--no-recursion", StringComparison.InvariantCultureIgnoreCase))
                    {
                        appConfiguration.DetectorConfiguration.Recursive = false;
                    }
                    else if (string.Equals(current, "-v", StringComparison.InvariantCultureIgnoreCase) || string.Equals(current, "--verbose", StringComparison.InvariantCultureIgnoreCase))
                    {
                        appConfiguration.Verbose = true;
                    }
                    else
                    {
                        throw new ArgumentException($"Unknown parameter {current}");
                    }
                }
                else
                {
                    paths.Add(current);
                }
            }

            appConfiguration.DetectorConfiguration.Paths = paths.ToArray();

            return appConfiguration;
        }

        public void PrintHelp()
        {
            System.Console.WriteLine("Files format detector");
            System.Console.WriteLine("Usage: FilesFormatDetector.Console.exe [OPTIONS] List of files or directories");
            System.Console.WriteLine();
            System.Console.WriteLine("Options:");
            System.Console.WriteLine(" -h,  --help:          Print help");
            System.Console.WriteLine(" -t,  --threads [N]:   Number of parallel threads (default is number of CPU cores)");
            System.Console.WriteLine(" -n,  --no-recursion:  Scan directories non recursively");
            System.Console.WriteLine(" -v,  --verbose:       Print summary about each file individually");
        }
    }
}
