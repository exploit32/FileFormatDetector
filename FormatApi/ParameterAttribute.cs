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
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
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

        /// <summary>
        /// For boolean types indicates that presence of flag means false value
        /// </summary>
        public bool IsInverted { get; }

        public ParameterAttribute(string key, string description, bool isInverted = false)
        {
            Key = key;
            Description = description;
            IsInverted = isInverted;
        }
    }
}
