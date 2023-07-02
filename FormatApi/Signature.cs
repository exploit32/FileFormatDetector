using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormatApi
{
    public class Signature
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
    }
}
