using FormatApi;
using System.Reflection;

namespace FileFormatDetector.Console
{
    /// <summary>
    /// Class that configures provided classes by parsing command line parameters
    /// </summary>
    internal class CommandLineParser
    {
        private DefaultParameter? _defaultParameter = null;

        private List<Parameter> _parameters = new List<Parameter>();

        /// <summary>
        /// Check for help request in parameters
        /// </summary>
        /// <param name="args">Parameters</param>
        /// <returns>True is help request was found</returns>
        public bool HelpRequested(string[] args)
        {
            return args.Any(a => a.Equals("--help", StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        /// Scan provided classes to look for properties, marked with <see cref="ParameterAttribute"/> or <see cref="DefaultParameterAttribute"/>
        /// </summary>
        /// <param name="objects">Collection of objects</param>
        public void Configure(IEnumerable<object> objects)
        {
            foreach (var obj in objects)
            {
                Configure(obj);
            }
        }

        /// <summary>
        /// Scan provided class to look for properties, marked with <see cref="ParameterAttribute"/> or <see cref="DefaultParameterAttribute"/>
        /// </summary>
        /// <param name="obj">Object to scan</param>
        /// <exception cref="ArgumentException">Exception is thrown if object defines parameter which was already declared by another object</exception>
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
                        throw new ArgumentException($"Detector {obj.GetType().Name} declares parameter {parameterAttribute.Key} which is already declared by detector {existing.TargetObject.GetType().Name}");

                    _parameters.Add(new Parameter(parameterAttribute.Key, parameterAttribute.Description, obj, prop)
                    {
                        IsFlag = prop.PropertyType == typeof(bool) || prop.PropertyType == typeof(bool?),
                        IsInverted = parameterAttribute.IsInverted,
                    });
                }

                var defaultParameterAttribute = prop.GetCustomAttribute<DefaultParameterAttribute>(true);

                if (defaultParameterAttribute != null)
                {
                    if (_defaultParameter != null)
                        throw new ArgumentException($"Only one default parameter is allowed. Object {obj.GetType().Name} declares default parameter, but default parameter is already declared by object {_defaultParameter.TargetObject.GetType().Name}");

                    if (!prop.PropertyType.IsAssignableTo(typeof(IEnumerable<string>)))
                        throw new ArgumentException($"Default parameter should be assignable to {nameof(IEnumerable<string>)}");

                    _defaultParameter = new DefaultParameter(obj, prop);
                }
            }
        }

        /// <summary>
        /// Parse command line arguments and write property values to provided classes
        /// </summary>
        /// <param name="args">Command line arguments</param>
        /// <exception cref="ArgumentException">Exception is thrown if unknown argument is found or argument syntax error</exception>
        public void Parse(string[] args)
        {
            int index = 0;

            while(index < args.Length)
            {
                string current = args[index++];

                if (current.StartsWith("--"))
                {
                    string currentTrimmed = current[2..];
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

        /// <summary>
        /// Print help with application parameters description
        /// </summary>
        public void PrintHelp()
        {
            System.Console.WriteLine("Files format detector");
            System.Console.WriteLine("Usage: FilesFormatDetector.Console.exe [OPTIONS] List of files or directories");
            System.Console.WriteLine();
            System.Console.WriteLine("Options:");
            System.Console.WriteLine(" -h,  --help:        Print help");

            var ownParameters = _parameters.Where(p => p.TargetObject.GetType().Assembly == typeof(Program).Assembly).ToList();

            if (ownParameters.Any())
                PrintParameters(ownParameters, false);

            var detectorParameters = _parameters.Where(p => p.TargetObject.GetType().Assembly != typeof(Program).Assembly).ToList();

            if (detectorParameters.Any())
            {
                System.Console.WriteLine();
                System.Console.WriteLine("Detector's options:");

                PrintParameters(detectorParameters, true);
            }
        }

        /// <summary>
        /// Print parameters description
        /// </summary>
        /// <param name="parameters">Collection of parameters</param>
        /// <param name="printClass">Print or skip class name that owns parameter</param>
        private void PrintParameters(IEnumerable<Parameter> parameters, bool printClass)
        {
            var paramsDescription = parameters.Select(p => (Name: $"--{p.Key}{(p.IsFlag ? "" : " (value)")}:", p.Description, Detector: p.TargetObject.GetType().Name)).ToList();

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

        /// <summary>
        /// Write parsed parameters back to owned classes
        /// </summary>
        private void ApplyParameters()
        {
            foreach (var parameter in _parameters)
            {
                if (parameter.ValueSet)
                {
                    if (parameter.ParameterType == typeof(string))
                    {
                        parameter.Property.SetValue(parameter.TargetObject, parameter.Value);
                    }
                    else if (parameter.ParameterType == typeof(bool) || parameter.ParameterType == typeof(bool?))
                    {
                        parameter.Property.SetValue(parameter.TargetObject, !parameter.IsInverted);
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
                    _defaultParameter.Property.SetValue(_defaultParameter.TargetObject, _defaultParameter.Values.ToArray());
                else
                    _defaultParameter.Property.SetValue(_defaultParameter.TargetObject, _defaultParameter.Values);
            }
        }

        /// <summary>
        /// Parse parameter of specified type and assing it's value to owner object's property
        /// </summary>
        /// <typeparam name="T">Parameter type</typeparam>
        /// <param name="parameter">Parameter description</param>
        /// <param name="tryParse">Try parse delegate</param>
        /// <exception cref="ArgumentNullException">Thrown if parameter value isn't set</exception>
        /// <exception cref="ArgumentException">Thrown if parameter isn't parsed correctly</exception>
        private void ParseAndSet<T>(Parameter parameter, TryParseDelegate<T> tryParse)
        {
            if (string.IsNullOrEmpty(parameter.Value))
                throw new ArgumentNullException($"Error parsing property {parameter.Key} value can't be null");

            if (tryParse(parameter.Value, out T parsed))
            {
                try
                {
                    parameter.Property.SetValue(parameter.TargetObject, parsed);
                }
                catch (TargetInvocationException ex) when(ex.InnerException != null)
                {
                    throw ex.InnerException;
                }
            }
            else
            {
                throw new ArgumentException($"Error parsing property {parameter.Key} for object {parameter.TargetObject.GetType().Name}");
            }
        }

        /// <summary>
        /// Internal representation of parameter
        /// </summary>
        class Parameter
        {
            /// <summary>
            /// Parameter key
            /// </summary>
            public string Key { get; init; }

            /// <summary>
            /// Parameter description
            /// </summary>
            public string Description { get; init; }

            /// <summary>
            /// Object that owns this parameter
            /// </summary>
            public object TargetObject { get; init; }

            /// <summary>
            /// Parameter type
            /// </summary>
            public Type ParameterType { get; init; }

            /// <summary>
            /// Owner object property info
            /// </summary>
            public PropertyInfo Property { get; init; }

            /// <summary>
            /// Indicates that parameter doesn't have value
            /// </summary>
            public bool IsFlag { get; init; }

            /// <summary>
            /// Indicates that flag parameter is inverted. Presence of flag means false value
            /// </summary>
            public bool IsInverted { get; init; }

            /// <summary>
            /// Parameter string value
            /// </summary>
            public string? Value { get; set; }

            /// <summary>
            /// Indicates that value is set
            /// </summary>
            public bool ValueSet { get; set; }

            public Parameter(string key, string description, object targetObject, PropertyInfo property)
            {
                Key = key;
                Description = description;
                TargetObject = targetObject;                
                Property = property;
                ParameterType = property.PropertyType;
            }

            public override string ToString()
            {
                return $"{Key}: {ParameterType.Name}";
            }
        }

        /// <summary>
        /// Internal representation of default parameter
        /// </summary>
        class DefaultParameter
        {
            /// <summary>
            /// Object that owns parameter
            /// </summary>
            public object TargetObject { get; init; }

            /// <summary>
            /// Parameter type
            /// </summary>
            public Type ParameterType { get; init; }

            /// <summary>
            /// Owner object property info
            /// </summary>
            public PropertyInfo Property { get; init; }

            /// <summary>
            /// List of values
            /// </summary>
            public List<string> Values { get; set; } = new List<string>();

            public DefaultParameter(object targetObject, PropertyInfo property)
            {
                TargetObject = targetObject;
                Property = property;
                ParameterType = property.PropertyType;
            }
        }
    }
}
