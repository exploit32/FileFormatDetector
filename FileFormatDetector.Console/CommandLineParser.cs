using FormatApi;
using System.Reflection;

namespace FileFormatDetector.Console
{
    internal class CommandLineParser
    {
        private DefaultParameter? _defaultParameter = null;

        private List<Parameter> _parameters = new List<Parameter>();

        public bool HelpRequested(string[] args)
        {
            return args.Any(a => a.Equals("-h", StringComparison.InvariantCultureIgnoreCase) || a.Equals("--help", StringComparison.InvariantCultureIgnoreCase));
        }

        public void Configure(IEnumerable<object> objects)
        {
            foreach (var obj in objects)
            {
                Configure(obj);
            }
        }

        public void Configure(object obj)
        {
            var properties = obj.GetType().GetProperties();

            foreach (var prop in properties)
            {
                var parameterAttribute = prop.GetCustomAttribute<ParameterAttribute>(true);

                if (parameterAttribute != null)
                {
                    var existing = _parameters.FirstOrDefault(p => p.Key.Equals(parameterAttribute.Key, StringComparison.InvariantCultureIgnoreCase));

                    if (existing != null)
                        throw new ArgumentException($"Detector {obj.GetType().Name} declares parameter {parameterAttribute.Key} which is already declared by detector {existing.Detector.GetType().Name}");

                    _parameters.Add(new Parameter()
                    {
                        Key = parameterAttribute.Key,
                        Description = parameterAttribute.Description,
                        Detector = obj,
                        Property = prop,
                        ParameterType = prop.PropertyType,
                        IsFlag = prop.PropertyType == typeof(bool) || prop.PropertyType == typeof(bool?),
                        IsInverted = parameterAttribute.IsInverted,
                    });
                }

                var defaultParameterAttribute = prop.GetCustomAttribute<DefaultParameterAttribute>(true);

                if (defaultParameterAttribute != null)
                {
                    if (_defaultParameter != null)
                        throw new ArgumentException($"Only one default parameter is allowed. Object {obj.GetType().Name} declares default parameter, but default parameter is already declared by object {_defaultParameter.Detector.GetType().Name}");

                    _defaultParameter = new DefaultParameter()
                    {
                        Detector = obj,
                        Property = prop,
                        ParameterType = prop.PropertyType,
                    };
                }
            }
        }

        public void Parse(string[] args)
        {
            int index = 0;

            while(index < args.Length)
            {
                string current = args[index++];

                if (current.StartsWith("--"))
                {
                    string currentTrimmed = current.Substring(2);
                    bool parameterSet = false;

                    foreach (var parameter in _parameters)  
                    {
                        if (currentTrimmed.Equals(parameter.Key, StringComparison.InvariantCultureIgnoreCase))
                        {
                            if (parameter.ValueSet)
                                throw new ArgumentException($"Value for parameter {currentTrimmed} was already set");

                            if (!parameter.IsFlag)
                            {
                                if (index < args.Length)
                                    parameter.Value = args[index++];
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
                    if (_defaultParameter != null)
                    {
                        _defaultParameter.Values.Add(current);
                    }
                }
            }

            ApplyParameters();
        }

        public void PrintHelp()
        {
            System.Console.WriteLine("Files format detector");
            System.Console.WriteLine("Usage: FilesFormatDetector.Console.exe [OPTIONS] List of files or directories");
            System.Console.WriteLine();
            System.Console.WriteLine("Options:");
            System.Console.WriteLine(" -h,  --help:        Print help");

            var ownParameters = _parameters.Where(p => p.Detector.GetType().Assembly == typeof(Program).Assembly).ToList();

            if (ownParameters.Any())
                PrintParameters(ownParameters, false);

            var detectorParameters = _parameters.Where(p => p.Detector.GetType().Assembly != typeof(Program).Assembly).ToList();

            if (detectorParameters.Any())
            {
                System.Console.WriteLine();
                System.Console.WriteLine("Detector's options:");

                PrintParameters(detectorParameters, true);
            }
        }

        private void PrintParameters(IEnumerable<Parameter> parameters, bool printClass)
        {
            var paramsDescription = parameters.Select(p => (Name: $"--{p.Key}{(p.IsFlag ? "" : " (value)")}:", Description: p.Description, Detector: p.Detector.GetType().Name)).ToList();

            int parameterNameLength = paramsDescription.Max(p => p.Name.Length);
            int parameterDescriptionLength = paramsDescription
                .SelectMany(p => p.Description.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                .Max(v => v.Length);

            int padding = 2;

            foreach (var parameter in paramsDescription)
            {
                var descriptionLines = parameter.Description.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                for (int i = 0; i < descriptionLines.Length; i++)
                {
                    if (i == 0)
                    {
                        System.Console.WriteLine(" {0}{1}{2}{3}{4}",
                            parameter.Name,
                            new string(' ', parameterNameLength - parameter.Name.Length + padding),
                            descriptionLines[i],
                            new string(' ', parameterDescriptionLength - descriptionLines[i].Length + padding),
                            printClass ? $"({parameter.Detector})" : String.Empty);
                    }
                    else
                    {
                        System.Console.WriteLine(" {0}{1}",
                            new string(' ', parameterNameLength + padding),
                            descriptionLines[i]);
                    }
                }
            }
        }

        private delegate bool TryParseDelegate<T>(string text, out T value);

        private void ApplyParameters()
        {
            foreach (var parameter in _parameters)
            {
                if (parameter.ValueSet)
                {
                    if (parameter.ParameterType == typeof(string))
                    {
                        parameter.Property.SetValue(parameter.Detector, parameter.Value);
                    }
                    else if (parameter.ParameterType == typeof(bool) || parameter.ParameterType == typeof(bool?))
                    {
                        parameter.Property.SetValue(parameter.Detector, !parameter.IsInverted);
                    }
                    else if (parameter.ParameterType == typeof(int) || parameter.ParameterType == typeof(int?))
                    {
                        ParseAndSet<int>(parameter, int.TryParse);
                    }
                    else if (parameter.ParameterType == typeof(long) || parameter.ParameterType == typeof(long?))
                    {
                        ParseAndSet<long>(parameter, long.TryParse);
                    }
                }
            }

            if (_defaultParameter != null)
            {
                if (_defaultParameter.ParameterType.IsArray)
                    _defaultParameter.Property.SetValue(_defaultParameter.Detector, _defaultParameter.Values.ToArray());
                else
                    _defaultParameter.Property.SetValue(_defaultParameter.Detector, _defaultParameter.Values);
            }
        }

        private void ParseAndSet<T>(Parameter parameter, TryParseDelegate<T> tryParse)
        {
            if (string.IsNullOrEmpty(parameter.Value))
                throw new ArgumentNullException($"Error parsing property {parameter.Key} value can't be null");

            if (tryParse(parameter.Value, out T parsed))
            {
                parameter.Property.SetValue(parameter.Detector, parsed);
            }
            else
            {
                throw new ArgumentException($"Error parsing property {parameter.Key} for detector {parameter.Detector.GetType().Name}");
            }
        }

        internal class Parameter
        {
            public string Key { get; init; }

            public string Description { get; init; }

            public object Detector { get; init; }

            public Type ParameterType { get; init; }

            public PropertyInfo Property { get; init; }

            public bool IsFlag { get; init; }

            public bool IsInverted { get; init; }

            public string? Value { get; set; }

            public bool ValueSet { get; set; }

            public override string ToString()
            {
                return $"{Key}: {ParameterType.Name}";
            }
        }

        internal class DefaultParameter
        {
            public object Detector { get; init; }

            public Type ParameterType { get; init; }

            public PropertyInfo Property { get; init; }

            public List<string> Values { get; set; } = new List<string>();
        }
    }
}
