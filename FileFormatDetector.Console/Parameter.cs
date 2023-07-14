using FormatApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FileFormatDetector.Console
{
    internal class Parameter
    {
        public string Key { get; init; }

        public string Description { get; init; }

        public object Detector { get; init; }

        public Type ParameterType { get; init; }

        public PropertyInfo Property { get; init; }

        public bool IsFlag { get; init; }

        public string? Value { get; set; }

        public bool ValueSet { get; set; }

        
    }
}
