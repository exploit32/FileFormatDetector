using FormatApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileFormatDetector.Console
{
    internal class Parameter
    {
        public ParameterDescription Description { get; set; }

        public IConfigurableDetector Detector { get; set; }

        public string? Value { get; set; }

        public bool ValueSet { get; set; }

        public Parameter(IConfigurableDetector detector, ParameterDescription detectorParameter)
        {
            Detector = detector;
            Description = detectorParameter;
        }
    }
}
