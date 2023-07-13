using FormatApi;
using System;
using System.Collections.Generic;
using System.Linq;
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
                    parameter.Detector.SetParameter(parameter.Description.Key, parameter.Value!);
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
                if (detector is IConfigurableDetector)
                {
                    var configurableDetector = (IConfigurableDetector)detector;

                    var detectorParameters = configurableDetector.GetParameters();

                    foreach (var parameter in detectorParameters)
                    {
                        var existing = parameters.FirstOrDefault(p => p.Description.Key.Equals(parameter.Key, StringComparison.InvariantCultureIgnoreCase));

                        if (existing != null)
                            throw new ArgumentException($"Detector {detector.GetType().Name} declares parameter {parameter.Key} which is already declared by detector {existing.Detector.GetType().Name}");

                        parameters.Add(new Parameter(configurableDetector, parameter));
                    }
                }
            }
        }
    }
}
