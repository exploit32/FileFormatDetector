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
            Value = value;
            Offset = offset;
        }

        public Signature(byte[] value) : this(value, 0) { }

        public Signature()
        {
        }        
    }
}
