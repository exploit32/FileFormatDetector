using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormatApi
{
    /// <summary>
    /// Attribute that indicates configurable parameter
    /// </summary>
    public class ParameterAttribute: Attribute
    {
        /// <summary>
        /// Parameter key. Will be used in command line arguments
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// Human-readable description of the parameter. Will be shown in help message
        /// </summary>
        public string Description { get; }

        public ParameterAttribute(string key, string description)
        {
            Key = key;
            Description = description;
        }
    }
}
