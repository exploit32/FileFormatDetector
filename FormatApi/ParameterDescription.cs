using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormatApi
{
    /// <summary>
    /// Description of detector's parameter
    /// </summary>
    public class ParameterDescription
    {
        /// <summary>
        /// Parameter key. Will be used in command line arguments
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// Human-readable description of the parameter. Will be shown in help message
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Indicates that parameter doesn't have value
        /// </summary>
        public bool IsFlag { get; }

        public ParameterDescription(string key, string description, bool isFlag)
        {
            Key = key;
            Description = description;
            IsFlag = isFlag;
        }
    }
}
