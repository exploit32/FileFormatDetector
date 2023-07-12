using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormatApi
{
    /// <summary>
    /// Base class for detectable formats
    /// </summary>
    public abstract class FormatSummary
    {
        /// <summary>
        /// Format short name
        /// </summary>
        public abstract string FormatName { get; }

        /// <summary>
        /// Array of available keys
        /// </summary>
        /// <returns></returns>
        public abstract string[] GetKeys();

        /// <summary>
        /// Property accessor
        /// </summary>
        /// <param name="key">Property key</param>
        /// <returns></returns>
        public abstract dynamic this[string key] { get; }

        /// <summary>
        /// Equality comparison method
        /// </summary>
        /// <param name="obj">Other object</param>
        /// <returns></returns>
        public abstract override bool Equals(object? obj);

        /// <summary>
        /// Hashcode calculation method
        /// </summary>
        /// <returns></returns>
        public abstract override int GetHashCode();
    }
}
