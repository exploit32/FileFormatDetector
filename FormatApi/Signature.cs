using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FormatApi
{
    public class Signature : IEquatable<Signature>
    {
        public byte[] Value { get; init; }

        public int Offset { get; init; }

        public Signature(byte[] value, int offset)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            if (value.Length == 0) throw new ArgumentException("Signature cannot be empty", nameof(value));
            if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset));

            Value = value;
            Offset = offset;
        }

        public Signature(byte[] value) : this(value, 0) { }

        public override bool Equals(object? obj) => Equals(obj as Signature);

        public bool Equals(Signature? other)
        {
            if (other is null)
                return false;

            if (Object.ReferenceEquals(this, other))
                return true;

            if (this.GetType() != other.GetType())
                return false;

            return (Offset == other.Offset)
                && (Value.SequenceEqual(other.Value));
        }

        public override int GetHashCode() => HashCode.Combine(Offset, Value.Aggregate((sum, current) => unchecked((byte)(sum + current * 13))));
    }
}
