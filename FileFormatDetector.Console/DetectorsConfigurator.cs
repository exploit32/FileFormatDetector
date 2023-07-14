using FormatApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FileFormatDetector.Console
{
    internal class DetectorsConfigurator
    {
        private readonly IEnumerable<IBinaryFormatDetector> _binaryFormats;
        private readonly IEnumerable<ITextFormatDetector> _textFormats;
        private readonly IEnumerable<ITextBasedFormatDetector> _textBasedFormats;

        public IEnumerable<Parameter> Parameters { get; private set; }

        public DetectorsConfigurator(IEnumerable<IBinaryFormatDetector> binaryFormats, IEnumerable<ITextFormatDetector> textFormats, IEnumerable<ITextBasedFormatDetector> textBasedFormats)
        {
            _binaryFormats = binaryFormats;
            _textFormats = textFormats;
            _textBasedFormats = textBasedFormats;

            Parameters = LoadDetectorsParameters();
        }

        public void ApplyParameters()
        {
            foreach (var parameter in Parameters)
            {
                if (parameter.ValueSet)
                {
                    if (parameter.ParameterType == typeof(string))
                    {
                        parameter.Property.SetValue(parameter.Detector, parameter.Value);
                    } 
                    else if (parameter.ParameterType == typeof(int) || parameter.ParameterType == typeof(int?))
                    {
                        ParseAndSet<int>(parameter, int.TryParse);
                    }
                    else if (parameter.ParameterType == typeof(long) || parameter.ParameterType == typeof(long?))
                    {
                        ParseAndSet<long>(parameter, long.TryParse);
                    }
                    else if (parameter.ParameterType == typeof(bool) || parameter.ParameterType == typeof(bool?))
                    {
                        ParseAndSet<bool>(parameter, bool.TryParse);
                    }
                }
            }
        }

        delegate bool TryParseDelegate<T>(string text, out T value);

        private void ParseAndSet<T>(Parameter parameter, TryParseDelegate<T> tryParse)
        {
            if (tryParse(parameter.Value, out T parsed))
            {
                parameter.Property.SetValue(parameter.Detector, parsed);
            }
            else
            {
                throw new ArgumentException($"Error parsing property {parameter.Key} for detector {parameter.Detector.GetType().Name}");
            }
        }
        

        private IEnumerable<Parameter> LoadDetectorsParameters()
        {
            List<Parameter> parameters = new List<Parameter>();
            
            AddDetectorsParameters(parameters, _binaryFormats);
            AddDetectorsParameters(parameters, _textFormats);
            AddDetectorsParameters(parameters, _textBasedFormats);

            return parameters;
        }

        private void AddDetectorsParameters<T>(List<Parameter> parameters, IEnumerable<T> detectors)
        {
            foreach (var detector in detectors)
            {
                var properties = detector.GetType().GetProperties();

                foreach (var prop in properties)
                {
                    var parameterAttribute = prop.GetCustomAttribute<ParameterAttribute>(true);

                    if (parameterAttribute != null)
                    {
                        var existing = parameters.FirstOrDefault(p => p.Key.Equals(parameterAttribute.Key, StringComparison.InvariantCultureIgnoreCase));

                        if (existing != null)
                            throw new ArgumentException($"Detector {detector.GetType().Name} declares parameter {parameterAttribute.Key} which is already declared by detector {existing.Detector.GetType().Name}");

                        parameters.Add(new Parameter()
                        {
                            Key = parameterAttribute.Key,
                            Description = parameterAttribute.Description,
                            Detector = detector,
                            Property = prop,
                            ParameterType = prop.PropertyType,
                            IsFlag = prop.PropertyType == typeof(bool) || prop.PropertyType == typeof(bool?),
                        });
                    }
                }
            }
        }
    }
}
