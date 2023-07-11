using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    internal class Utilities
    {
        private const string ResourcesPrefix = "Tests.Samples.binary_samples.";

        public static Stream GetBinaryStream(string name)
        {
            var result = typeof(Utilities).Assembly.GetManifestResourceStream(ResourcesPrefix + name);
            
            if (result == null)
                throw new FileNotFoundException($"Could not find resource '{name}'.");

            return result;
        }        
    }
}
