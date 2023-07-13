using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormatApi
{
    /// <summary>
    /// Interface that provides ability to configure detectors
    /// </summary>
    public interface IConfigurableDetector
    {
        /// <summary>
        /// Request for all detector's parameters' description
        /// </summary>
        /// <returns>Detector's parameters</returns>
        IEnumerable<ParameterDescription> GetParameters();

        /// <summary>
        /// Apply parameter value
        /// </summary>
        /// <param name="key">Parameter key from description</param>
        /// <param name="value">Parameter value or empty string for flag parameters</param>
        void SetParameter(string key, string value);
    }
}
