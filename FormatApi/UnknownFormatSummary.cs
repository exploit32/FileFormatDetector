using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormatApi
{
    public class UnknownFormatSummary : FormatSummary, IEquatable<UnknownFormatSummary>
    {
        private static string[] Keys = Array.Empty<string>();

        public override dynamic this[string key] => throw new KeyNotFoundException($"Key {key} is not supported");

        public override string FormatName => "Unknown";

        public override bool Equals(object? obj) => this.Equals(obj as UnknownFormatSummary);

        public bool Equals(UnknownFormatSummary? other)
        {
            if (other is null)
                return false;

            if (Object.ReferenceEquals(this, other))
                return true;

            if (this.GetType() != other.GetType())
                return false;

            return true;
        }

        public override int GetHashCode() => HashCode.Combine(FormatName);

        public override string[] GetKeys() => Keys;

        public override string ToString()
        {
            return FormatName;
        }
    }
}
