using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormatApi
{
    /// <summary>
    /// Attribute that indicates default parameter
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DefaultParameterAttribute: Attribute
    {
    }
}
