using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormatApi
{
    public abstract class FormatSummary
    {
        public abstract string FormatName { get; }

        public abstract string[] GetKeys();

        public abstract dynamic this[string key] { get; }

        public abstract override bool Equals(object? obj);

        public abstract override int GetHashCode();
    }
}
