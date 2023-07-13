using FileFormatDetector.Core;
using System.Reflection.Metadata;


namespace FileFormatDetector.Console
{
    internal class CommandLineParser
    {
        private readonly string[] _args;
        private readonly IEnumerable<Parameter> _detectorParameters;

        public CommandLineParser(string[] args, IEnumerable<Parameter> detectorParameters)
        {
            _args = args;
            _detectorParameters = detectorParameters;
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
                            {
                                if (threadsCount <= 0)
                                    throw new ArgumentException("Number of threads should be greater than 0");

                                appConfiguration.DetectorConfiguration.Threads = threadsCount;
                            }
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
                    else if (current.StartsWith("--"))
                    {
                        string currentTrimmed = current.Substring(2);
                        bool parameterSet = false;

                        foreach (var parameter in _detectorParameters)  
                        {
                            if (currentTrimmed.Equals(parameter.Description.Key, StringComparison.InvariantCultureIgnoreCase))
                            {
                                if (parameter.ValueSet)
                                    throw new ArgumentException($"Value for parameter {currentTrimmed} was already set");

                                if (!parameter.Description.IsFlag)
                                {
                                    if (index < _args.Length)
                                        parameter.Value = _args[index++];
                                    else
                                        throw new ArgumentException($"Error parsing {currentTrimmed} parameter: value is missing");
                                }

                                parameter.ValueSet = true;
                                parameterSet = true;
                                break;
                            }
                        }

                        if (!parameterSet)
                            throw new ArgumentException($"Unknown parameter {current}");
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

            appConfiguration.Paths = paths.ToArray();

            return appConfiguration;
        }

        public void PrintHelp()
        {
            System.Console.WriteLine("Files format detector");
            System.Console.WriteLine("Usage: FilesFormatDetector.Console.exe [OPTIONS] List of files or directories");
            System.Console.WriteLine();
            System.Console.WriteLine("Options:");
            System.Console.WriteLine(" -h,  --help:          Print help");
            System.Console.WriteLine(" -t,  --threads (N):   Number of parallel threads (default is number of CPU cores)");
            System.Console.WriteLine(" -n,  --no-recursion:  Scan directories non recursively");
            System.Console.WriteLine(" -v,  --verbose:       Print summary about each file individually");

            if (_detectorParameters.Any())
            {
                System.Console.WriteLine();
                System.Console.WriteLine("Detector's options:");
                
                var paramsDescription = _detectorParameters.Select(p => (Name: $"--{p.Description.Key}{(p.Description.IsFlag ? "" : " (value)")}:", Description: p.Description.Description, Detector: p.Detector.GetType().Name)).ToList();

                int parameterNameLength = paramsDescription.Max(p => p.Name.Length);
                int parameterDescriptionLength = paramsDescription.Max(p => p.Description.Length);

                int padding = 3;

                foreach (var parameter in paramsDescription)
                {
                    System.Console.WriteLine("{0}{1}{2}{3}({4})",
                        parameter.Name,
                        new string(' ', parameterNameLength - parameter.Name.Length + padding),
                        parameter.Description,
                        new string(' ', parameterDescriptionLength - parameter.Description.Length + padding),
                        parameter.Detector);
                }
            }
        }
    }
}
